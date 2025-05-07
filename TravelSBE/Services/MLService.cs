using Microsoft.ML;
using Microsoft.ML.Trainers;
using TravelSBE.Data;
using TravelSBE.Models.ML;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using TravelSBE.Entity;
using TravelSBE.Utils;

namespace TravelSBE.Services
{
    public interface IMLService
    {
        Task TrainModelAsync();
        Task<List<RecommendedObjectiveDto>> GetRecommendedObjectivesAsync(int objectiveId, int count = 5);
    }

    public class MLService : IMLService
    {
        private readonly ApplicationDbContext _context;
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private PredictionEngine<ObjectiveFeatures, ClusterPrediction> _predictionEngine;
        private readonly IConfiguration _configuration;

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


                var trainingData = objectives.Select(o => new ObjectiveFeatures
                {
                    ObjectiveId = o.Id,
                    TypeId = (float)(o.Type ?? 0),
                    AverageRating = (float)(o.Reviews.Any() ? o.Reviews.Average(r => r.Raiting) : 0),
                    Price = ParsePrice(o.Pret),
                    ClusterId = o.ClusterId ?? 0
                }).ToList();

                var pipeline = _mlContext.Transforms.Concatenate(
                        "Features",
                        nameof(ObjectiveFeatures.TypeId),
                        nameof(ObjectiveFeatures.AverageRating),
                        nameof(ObjectiveFeatures.Price))
                    .Append(_mlContext.Transforms.NormalizeMinMax("Features"))
                    .Append(_mlContext.Clustering.Trainers.KMeans(
                      numberOfClusters: Math.Max(2, Math.Min(3, objectives.Count / 2)),
                        featureColumnName: "Features"));

                var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);
                _model = pipeline.Fit(dataView);

                _predictionEngine = _mlContext.Model.CreatePredictionEngine<ObjectiveFeatures, ClusterPrediction>(_model);

                foreach (var objective in objectives)
                {
                    var features = trainingData.First(f => f.ObjectiveId == objective.Id);
                    var prediction = _predictionEngine.Predict(features);
                    objective.ClusterId = (int)prediction.PredictedClusterId;
                }

                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<List<RecommendedObjectiveDto>> GetRecommendedObjectivesAsync(int objectiveId, int count = 5)
        {
            var objective = await _context.Objectives
                .FirstOrDefaultAsync(o => o.Id == objectiveId);

            if (objective == null || !objective.ClusterId.HasValue)
                return new List<RecommendedObjectiveDto>();

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
                        $"{_configuration["BaseUrl"]}/uploads/images/{o.Images.First().FilePath}" : 
                        null,
                    AverageRating = o.Reviews.Any() ? o.Reviews.Average(r => r.Raiting) : null
                })
                .ToListAsync();

            return similarObjectives;
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