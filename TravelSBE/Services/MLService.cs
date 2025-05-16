using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using TravelSBE.Data;
using TravelSBE.Entity;
using TravelSBE.Utils;

namespace TravelSBE.Services
{
    public interface IMLService
    {
        Task TrainModelAsync();
        Task<ServiceResult<List<RecommendedObjectiveDto>>> GetRecommendedObjectivesAsync(int objectiveId, int count = 4);
        Task<ServiceResult<List<ObjectiveVisualizationDto>>> GetObjectivesForVisualizationAsync();
    }

    public class MLService : IMLService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private const int K = 5; 
        private const int MAX_ITERATIONS = 100;
        private readonly Point _referencePoint;

        private class ClusteringPoint
        {
            public Objective Objective { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
        }

        public MLService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _referencePoint = new Point(26.1025, 44.4268) { SRID = 4326 };
        }

        private double CalculateDistanceFromReference(Point? location)
        {
            if (location == null)
                return 0;

            return location.Distance(_referencePoint) * 111000; 
        }

        public async Task TrainModelAsync()
        {
            try
            {
                var objectives = await _context.Objectives
                    .Include(o => o.ObjectiveType)
                    .ToListAsync();

                var points = objectives.Select(o => new ClusteringPoint
                {
                    Objective = o,
                    X = o.Location?.X ?? 0,  
                    Y = o.Location?.Y ?? 0  
                }).ToList();

                var normalizedPoints = await NormalizePoints(points);

                var clusters = ApplyKMeans(normalizedPoints);

                for (int i = 0; i < objectives.Count; i++)
                {
                    objectives[i].ClusterId = clusters[i];
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<List<(double x, double y)>> NormalizePoints(List<ClusteringPoint> points)
        {
            var normalizedPoints = new List<(double x, double y)>();
            
            var xValues = points.Select(p => p.X).ToList();
            var yValues = points.Select(p => p.Y).ToList();
            
            var minX = xValues.Min();
            var maxX = xValues.Max();
            var minY = yValues.Min();
            var maxY = yValues.Max();

            foreach (var point in points)
            {
                var normalizedX = (point.X - minX) / (maxX - minX);
                var normalizedY = (point.Y - minY) / (maxY - minY);
                
                normalizedPoints.Add((normalizedX, normalizedY));
            }

            return normalizedPoints;
        }

        private List<int> ApplyKMeans(List<(double x, double y)> points)
        {
            var centroids = new List<(double x, double y)>();
            var random = new Random();
            
            for (int i = 0; i < K; i++)
            {
                var randomIndex = random.Next(points.Count);
                centroids.Add(points[randomIndex]);
            }

            var clusters = new List<int>();
            var oldClusters = new List<int>();
            var iterations = 0;

            do
            {
                clusters.Clear();
                foreach (var point in points)
                {
                    var minDistance = double.MaxValue;
                    var cluster = 0;

                    for (int i = 0; i < K; i++)
                    {
                        var distance = CalculateDistance(point, centroids[i]);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            cluster = i;
                        }
                    }
                    clusters.Add(cluster);
                }

                for (int i = 0; i < K; i++)
                {
                    var clusterPoints = points.Where((p, index) => clusters[index] == i).ToList();
                    if (clusterPoints.Any())
                    {
                        var newX = clusterPoints.Average(p => p.x);
                        var newY = clusterPoints.Average(p => p.y);
                        centroids[i] = (newX, newY);
                    }
                }

                iterations++;
            } while (!AreClustersEqual(clusters, oldClusters) && iterations < MAX_ITERATIONS);

            return clusters;
        }

        private bool AreClustersEqual(List<int> clusters1, List<int> clusters2)
        {
            if (clusters1.Count != clusters2.Count) return false;
            return !clusters1.Where((t, i) => t != clusters2[i]).Any();
        }

        private double CalculateDistance((double x, double y) p1, (double x, double y) p2)
        {
            return Math.Sqrt(Math.Pow(p2.x - p1.x, 2) + Math.Pow(p2.y - p1.y, 2));
        }

        public async Task<ServiceResult<List<RecommendedObjectiveDto>>> GetRecommendedObjectivesAsync(int objectiveId, int count = 4)
        {
            var result = new ServiceResult<List<RecommendedObjectiveDto>>();
            
            var objective = await _context.Objectives
                .Include(o => o.ObjectiveType)
                .FirstOrDefaultAsync(o => o.Id == objectiveId);

            if (objective == null || !objective.ClusterId.HasValue || objective.Location == null)
                return result;

            var clusterObjectives = await _context.Objectives
                .Include(o => o.Reviews)
                .Include(o => o.Images)
                .Include(o => o.ObjectiveType)
                .Where(o => o.ClusterId == objective.ClusterId && o.Id != objectiveId && o.Location != null)
                .ToListAsync();

            var objectivesWithScore = clusterObjectives
                .Select(o => new
                {
                    Objective = o,
                    Distance = o.Location.Distance(objective.Location) * 111000,
                    TypeSimilarity = o.Type == objective.Type? 1.0 : 0.5
                })
                .Select(x => new
                {
                    Objective = x.Objective,
                    Score = CalculateRecommendationScore(x.Distance, x.TypeSimilarity)
                })
                .OrderByDescending(x => x.Score)
                .Take(count)
                .Select(x => new RecommendedObjectiveDto
                {
                    Id = x.Objective.Id,
                    Name = x.Objective.Name,
                    City = x.Objective.City,
                    FirstImageUrl = x.Objective.Images.Any() ? 
                        ImageHelper.GetFirstImageURL(x.Objective.Images): 
                        null,
                    AverageRating = x.Objective.Reviews.Any() ? 
                        x.Objective.Reviews.Average(r => r.Raiting) : 
                        null,
                    Distance = x.Objective.Location.Distance(objective.Location) * 111000,
                    ObjectiveType = x.Objective.ObjectiveType?.Name
                })
                .ToList();

            result.Result = objectivesWithScore;
            return result;
        }

        private double CalculateRecommendationScore(double distance, double typeSimilarity)
        {
            // Normalizăm distanța între 0 și 1 (0 fiind cea mai apropiată)
            double normalizedDistance = Math.Min(distance / 5000, 1.0); // Considerăm 5km ca distanță maximă
            
            // Calculăm scorul final (mai mare = mai bun)
            // 60% pentru distanță, 40% pentru similaritatea tipului
            return (1 - normalizedDistance) * 0.6 + typeSimilarity * 0.4;
        }

        public async Task<ServiceResult<List<ObjectiveVisualizationDto>>> GetObjectivesForVisualizationAsync()
        {
            var result = new ServiceResult<List<ObjectiveVisualizationDto>>();
            
            var objectives = await _context.Objectives
                .ToListAsync();

            var visualizationData = objectives.Select(o => new ObjectiveVisualizationDto
            {
                Id = o.Id,
                Name = o.Name,
                X = o.Location?.X ?? 0,  
                Y = o.Location?.Y ?? 0,  
                DistanceFromCenter = CalculateDistanceFromReference(o.Location),
                ClusterId = o.ClusterId ?? -1
            }).ToList();

            result.Result = visualizationData;
            return result;
        }
    }

    public class RecommendedObjectiveDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string FirstImageUrl { get; set; }
        public double? AverageRating { get; set; }
        public double Distance { get; set; }
        public string ObjectiveType { get; set; }
    }

    public class ObjectiveVisualizationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double X { get; set; } 
        public double Y { get; set; }  
        public double DistanceFromCenter { get; set; } 
        public int ClusterId { get; set; }
    }
} 