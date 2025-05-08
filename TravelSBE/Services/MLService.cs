using Microsoft.ML;
using Microsoft.ML.Trainers;
using TravelSBE.Data;
using TravelSBE.Models.ML;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using TravelSBE.Entity;
using TravelSBE.Utils;
using System.Collections.Generic;

namespace TravelSBE.Services
{
    public interface IMLService
    {
        Task TrainModelAsync();
        Task<ServiceResult<List<RecommendedObjectiveDto>>> GetRecommendedObjectivesAsync(int objectiveId, int count = 4);
    }

    public class MLService : IMLService
    {
        private readonly ApplicationDbContext _context;
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private PredictionEngine<ObjectiveFeatures, ClusterPrediction> _predictionEngine;
        private readonly IConfiguration _configuration;
        private const int K = 5; // mărim numărul de vecini
        private const double DISTANCE_THRESHOLD = 0.5; // mărim distanța (în grade)
        private const double SIMILARITY_THRESHOLD = 0.6; // relaxăm similaritatea
        private const int MAX_CLUSTERS = 10; // numărul maxim de clustere dorit

        public MLService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _mlContext = new MLContext();
            _configuration = configuration;
        }

        public async Task TrainModelAsync()
        {
            try
            {
                var objectives = await _context.Objectives
                    .Include(o => o.Reviews)
                    .Include(o => o.ObjectiveType)
                    .ToListAsync();

                // Resetăm toate clusterId-urile
                foreach (var obj in objectives)
                {
                    obj.ClusterId = null;
                }

                int currentClusterId = 1;
                var processedObjectives = new HashSet<int>();
                var clusterCenters = new Dictionary<int, Point>();

                // Prima fază: creăm clusterele inițiale
                foreach (var objective in objectives.Where(o => o.Location != null))
                {
                    if (processedObjectives.Contains(objective.Id))
                        continue;

                    var nearbyObjectives = await _context.Objectives
                        .Where(o => o.Id != objective.Id && !processedObjectives.Contains(o.Id))
                        .Where(o => o.Location != null)
                        .Where(o => o.Location.Distance(objective.Location) <= DISTANCE_THRESHOLD)
                        .Where(o => o.Type == objective.Type)
                        .Select(o => new
                        {
                            Objective = o,
                            Distance = o.Location.Distance(objective.Location)
                        })
                        .OrderBy(n => n.Distance)
                        .Take(K * 2)
                        .ToListAsync();

                    var nearestNeighbors = nearbyObjectives
                        .Select(n => new
                        {
                            n.Objective,
                            n.Distance,
                            Similarity = CalculatePriceSimilarity(
                                ParsePrice(objective.Pret),
                                ParsePrice(n.Objective.Pret))
                        })
                        .Where(n => n.Similarity >= SIMILARITY_THRESHOLD)
                        .Take(K)
                        .ToList();

                    // Atribuim clusterId chiar dacă nu găsim vecini
                    objective.ClusterId = currentClusterId;
                    processedObjectives.Add(objective.Id);
                    clusterCenters[currentClusterId] = objective.Location;

                    if (nearestNeighbors.Any())
                    {
                        foreach (var neighbor in nearestNeighbors)
                        {
                            neighbor.Objective.ClusterId = currentClusterId;
                            processedObjectives.Add(neighbor.Objective.Id);
                        }
                    }

                    currentClusterId++;
                }

                // A doua fază: atribuim clustere pentru obiectivele rămase
                foreach (var objective in objectives.Where(o => !processedObjectives.Contains(o.Id)))
                {
                    if (objective.Location == null)
                    {
                        // Pentru obiectivele fără locație, le atribuim un cluster nou
                        objective.ClusterId = currentClusterId++;
                        continue;
                    }

                    // Găsim cel mai apropiat cluster
                    var nearestCluster = clusterCenters
                        .OrderBy(c => c.Value.Distance(objective.Location))
                        .FirstOrDefault();

                    if (nearestCluster.Key != 0)
                    {
                        objective.ClusterId = nearestCluster.Key;
                    }
                    else
                    {
                        // Dacă nu găsim un cluster apropiat, creăm unul nou
                        objective.ClusterId = currentClusterId;
                        clusterCenters[currentClusterId] = objective.Location;
                        currentClusterId++;
                    }
                    processedObjectives.Add(objective.Id);
                }

                // A treia fază: combinăm clusterele dacă sunt prea multe
                if (clusterCenters.Count > MAX_CLUSTERS)
                {
                    var clustersToMerge = new List<(int cluster1, int cluster2)>();
                    
                    // Găsim clusterele care ar trebui combinate
                    foreach (var cluster1 in clusterCenters)
                    {
                        foreach (var cluster2 in clusterCenters)
                        {
                            if (cluster1.Key >= cluster2.Key) continue;

                            if (cluster1.Value.Distance(cluster2.Value) <= DISTANCE_THRESHOLD)
                            {
                                clustersToMerge.Add((cluster1.Key, cluster2.Key));
                            }
                        }
                    }

                    // Combinăm clusterele
                    foreach (var merge in clustersToMerge)
                    {
                        var objectivesToUpdate = objectives.Where(o => o.ClusterId == merge.cluster2);
                        foreach (var obj in objectivesToUpdate)
                        {
                            obj.ClusterId = merge.cluster1;
                        }
                    }
                }

                // Verificare finală: asigurăm-ne că toate obiectivele au un ClusterId
                foreach (var objective in objectives.Where(o => o.ClusterId == null))
                {
                    objective.ClusterId = currentClusterId++;
                }

                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        private double CalculatePriceSimilarity(float price1, float price2)
        {
            if (price1 == 0 && price2 == 0)
                return 1.0;

            if (price1 == 0 || price2 == 0)
                return 0.0;

            var maxPrice = Math.Max(price1, price2);
            var minPrice = Math.Min(price1, price2);
            return minPrice / maxPrice;
        }

        public async Task<ServiceResult<List<RecommendedObjectiveDto>>> GetRecommendedObjectivesAsync(int objectiveId, int count = 5)

        {
            var result = new ServiceResult<List<RecommendedObjectiveDto>>();
            var objective = await _context.Objectives
                .FirstOrDefaultAsync(o => o.Id == objectiveId);

            if (objective == null || !objective.ClusterId.HasValue)
                return result;

            var similarObjectives = await _context.Objectives
                .Include(o => o.Reviews)
                .Include(o => o.Images)
                .Where(o => o.ClusterId == objective.ClusterId && o.Id != objectiveId)
                .OrderByDescending(o => o.Reviews.Average(r => r.Raiting))
                .Take(count)
                .Select(o => new RecommendedObjectiveDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    City = o.City,
                    FirstImageUrl = o.Images.Any() ? 
                        ImageHelper.GetFirstImageURL(o.Images): 
                        null,
                    AverageRating = o.Reviews.Any() ? o.Reviews.Average(r => r.Raiting) : null
                })
                .ToListAsync();
            result.Result = similarObjectives;
            return result;
        }

        private float ParsePrice(string? pret)
        {
            if (string.IsNullOrEmpty(pret))
                return 0;

            var match = System.Text.RegularExpressions.Regex.Match(pret, @"\d+");
            if (match.Success && float.TryParse(match.Value, out float result))
                return result;

            return 0;
        }

    }

    public class ClusterPrediction
    {
        public uint PredictedClusterId { get; set; }
        public float[] Distances { get; set; }
    }
} 