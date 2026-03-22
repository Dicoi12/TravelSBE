using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        Task UpdateClusterNeighborsAsync();
        Task<ServiceResult<List<RecommendedObjectiveDto>>> GetNearestNeighborsAsync(int objectiveId, int count = 4);
        Task<ServiceResult<ClusterAnalysisDto>> GetClusterAnalysisAsync();
    }

    public class MLService : IMLService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MLService> _logger;

        private readonly int _k;
        private readonly int _maxIterations;
        private readonly Point _referencePoint;
        private readonly double _locationWeight;
        private readonly double _typeWeight;
        private readonly double _ratingWeight;
        private readonly double _priceWeight;

        // Cache: skip training if already done in this process lifetime and data unchanged
        private static bool _isTrained = false;
        private static int _lastTrainedObjectiveCount = 0;

        private class ClusteringPoint
        {
            public Objective Objective { get; set; } = null!;
            public double X { get; set; }
            public double Y { get; set; }
            public double AverageRating { get; set; }
            public double Price { get; set; }
            public int TypeId { get; set; }
        }

        public MLService(ApplicationDbContext context, IConfiguration configuration, ILogger<MLService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;

            var ml = configuration.GetSection("MachineLearning");
            _k = ml.GetValue<int>("K", 5);
            _maxIterations = ml.GetValue<int>("MaxIterations", 100);
            _locationWeight = ml.GetValue<double>("LocationWeight", 0.6);
            _typeWeight = ml.GetValue<double>("TypeWeight", 0.2);
            _ratingWeight = ml.GetValue<double>("RatingWeight", 0.1);
            _priceWeight = ml.GetValue<double>("PriceWeight", 0.1);

            var lon = ml.GetValue<double>("ReferencePointLongitude", 26.1025);
            var lat = ml.GetValue<double>("ReferencePointLatitude", 44.4268);
            _referencePoint = new Point(lon, lat) { SRID = 4326 };
        }

        private double CalculateDistanceFromReference(Point? location)
        {
            if (location == null) return 0;
            return location.Distance(_referencePoint) * 111000;
        }

        public async Task TrainModelAsync()
        {
            try
            {
                var objectives = await _context.Objectives
                    .Include(o => o.ObjectiveType)
                    .ToListAsync();

                // Skip retraining if data hasn't changed since last run
                if (_isTrained && _lastTrainedObjectiveCount == objectives.Count)
                {
                    _logger.LogInformation("ML model already trained for {Count} objectives, skipping.", objectives.Count);
                    return;
                }

                _logger.LogInformation("Training ML model with {Count} objectives, K={K}.", objectives.Count, _k);

                var points = objectives.Select(o => new ClusteringPoint
                {
                    Objective = o,
                    X = o.Location?.X ?? 0,
                    Y = o.Location?.Y ?? 0,
                    AverageRating = o.Reviews.Any() ? o.Reviews.Average(r => r.Raiting) : 0,
                    Price = !string.IsNullOrEmpty(o.Pret) && int.TryParse(o.Pret, out var p) ? p : 0,
                    TypeId = o.Type ?? 0
                }).ToList();

                var normalizedPoints = NormalizePoints(points);
                var clusters = ApplyKMeans(normalizedPoints);

                for (int i = 0; i < objectives.Count; i++)
                {
                    objectives[i].ClusterId = clusters[i];
                }

                await _context.SaveChangesAsync();

                _isTrained = true;
                _lastTrainedObjectiveCount = objectives.Count;

                _logger.LogInformation("ML model training complete.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during ML model training.");
                throw;
            }
        }

        private List<(double x, double y, double rating, double price, double type)> NormalizePoints(List<ClusteringPoint> points)
        {
            var normalizedPoints = new List<(double x, double y, double rating, double price, double type)>();

            var xValues = points.Select(p => p.X).ToList();
            var yValues = points.Select(p => p.Y).ToList();
            var ratingValues = points.Select(p => p.AverageRating).ToList();
            var priceValues = points.Select(p => p.Price).ToList();
            var typeValues = points.Select(p => (double)p.TypeId).ToList();

            var minX = xValues.Min(); var maxX = xValues.Max();
            var minY = yValues.Min(); var maxY = yValues.Max();
            var minRating = ratingValues.Min(); var maxRating = ratingValues.Max();
            var minPrice = priceValues.Min(); var maxPrice = priceValues.Max();
            var minType = typeValues.Min(); var maxType = typeValues.Max();

            if (maxX == minX) maxX = minX + 1;
            if (maxY == minY) maxY = minY + 1;
            if (maxRating == minRating) maxRating = minRating + 1;
            if (maxPrice == minPrice) maxPrice = minPrice + 1;
            if (maxType == minType) maxType = minType + 1;

            foreach (var point in points)
            {
                var normalizedX = ((point.X - minX) / (maxX - minX)) * _locationWeight;
                var normalizedY = ((point.Y - minY) / (maxY - minY)) * _locationWeight;
                var normalizedRating = ((point.AverageRating - minRating) / (maxRating - minRating)) * _ratingWeight;
                var normalizedPrice = ((point.Price - minPrice) / (maxPrice - minPrice)) * _priceWeight;
                var normalizedType = ((point.TypeId - minType) / (maxType - minType)) * _typeWeight;

                normalizedPoints.Add((normalizedX, normalizedY, normalizedRating, normalizedPrice, normalizedType));
            }

            return normalizedPoints;
        }

        private List<int> ApplyKMeans(List<(double x, double y, double rating, double price, double type)> points)
        {
            var centroids = new List<(double x, double y, double rating, double price, double type)>();
            var random = new Random();

            for (int i = 0; i < _k; i++)
            {
                centroids.Add(points[random.Next(points.Count)]);
            }

            var clusters = new List<int>();
            var oldClusters = new List<int>();
            var iterations = 0;

            do
            {
                oldClusters = new List<int>(clusters);
                clusters.Clear();

                foreach (var point in points)
                {
                    var minDistance = double.MaxValue;
                    var cluster = 0;

                    for (int i = 0; i < _k; i++)
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

                for (int i = 0; i < _k; i++)
                {
                    var clusterPoints = points.Where((p, index) => clusters[index] == i).ToList();
                    if (clusterPoints.Any())
                    {
                        centroids[i] = (
                            clusterPoints.Average(p => p.x),
                            clusterPoints.Average(p => p.y),
                            clusterPoints.Average(p => p.rating),
                            clusterPoints.Average(p => p.price),
                            clusterPoints.Average(p => p.type)
                        );
                    }
                }

                iterations++;
            } while (!AreClustersEqual(clusters, oldClusters) && iterations < _maxIterations);

            return clusters;
        }

        private bool AreClustersEqual(List<int> clusters1, List<int> clusters2)
        {
            if (clusters1.Count != clusters2.Count) return false;
            return !clusters1.Where((t, i) => t != clusters2[i]).Any();
        }

        private double CalculateDistance(
            (double x, double y, double rating, double price, double type) p1,
            (double x, double y, double rating, double price, double type) p2)
        {
            return Math.Sqrt(
                Math.Pow(p2.x - p1.x, 2) +
                Math.Pow(p2.y - p1.y, 2) +
                Math.Pow(p2.rating - p1.rating, 2) +
                Math.Pow(p2.price - p1.price, 2) +
                Math.Pow(p2.type - p1.type, 2)
            );
        }

        public async Task<ServiceResult<List<RecommendedObjectiveDto>>> GetRecommendedObjectivesAsync(int objectiveId, int count = 4)
        {
            var result = new ServiceResult<List<RecommendedObjectiveDto>>();

            var objective = await _context.Objectives.FirstOrDefaultAsync(o => o.Id == objectiveId);

            if (objective == null || !objective.ClusterId.HasValue || objective.Location == null)
                return result;

            var clusterObjectives = await _context.Objectives
                .Include(o => o.Reviews)
                .Include(o => o.Images)
                .Where(o => o.ClusterId == objective.ClusterId && o.Id != objectiveId && o.Location != null)
                .ToListAsync();

            var objectivesWithDistance = clusterObjectives
                .Select(o => new
                {
                    Objective = o,
                    Distance = o.Location!.Distance(objective.Location) * 111000
                })
                .OrderBy(x => x.Distance)
                .Take(count)
                .Select(x => new RecommendedObjectiveDto
                {
                    Id = x.Objective.Id,
                    Name = x.Objective.Name,
                    City = x.Objective.City,
                    FirstImageUrl = x.Objective.Images.Any() ? ImageHelper.GetFirstImageURL(x.Objective.Images) : null,
                    AverageRating = x.Objective.Reviews.Any() ? x.Objective.Reviews.Average(r => r.Raiting) : null,
                    Distance = x.Distance
                })
                .ToList();

            result.Result = objectivesWithDistance;
            return result;
        }

        public async Task<ServiceResult<List<ObjectiveVisualizationDto>>> GetObjectivesForVisualizationAsync()
        {
            var result = new ServiceResult<List<ObjectiveVisualizationDto>>();

            var objectives = await _context.Objectives
                .Include(o => o.Reviews)
                .Include(o => o.ObjectiveType)
                .ToListAsync();

            var visualizationData = objectives.Select(o => new ObjectiveVisualizationDto
            {
                Id = o.Id,
                Name = o.Name,
                X = o.Location?.X ?? 0,
                Y = o.Location?.Y ?? 0,
                DistanceFromCenter = CalculateDistanceFromReference(o.Location),
                ClusterId = o.ClusterId ?? -1,
                AverageRating = o.Reviews.Any() ? o.Reviews.Average(r => r.Raiting) : 0,
                Price = !string.IsNullOrEmpty(o.Pret) && int.TryParse(o.Pret, out var p) ? p : 0,
                TypeName = o.ObjectiveType?.Name ?? "Unknown"
            }).ToList();

            result.Result = visualizationData;
            return result;
        }

        private double CalculateSimilarityScore(Objective obj1, Objective obj2)
        {
            var locationScore = 1.0 - (obj1.Location!.Distance(obj2.Location!) * 111000 / 100000);
            var ratingScore = obj1.Reviews.Any() && obj2.Reviews.Any()
                ? 1.0 - Math.Abs(obj1.Reviews.Average(r => r.Raiting) - obj2.Reviews.Average(r => r.Raiting)) / 5.0
                : 0.5;
            var priceScore = !string.IsNullOrEmpty(obj1.Pret) && !string.IsNullOrEmpty(obj2.Pret)
                ? 1.0 - Math.Abs(double.Parse(obj1.Pret) - double.Parse(obj2.Pret)) / 1000.0
                : 0.5;
            var typeScore = obj1.Type == obj2.Type ? 1.0 : 0.0;

            return (locationScore * _locationWeight) + (ratingScore * _ratingWeight) + (priceScore * _priceWeight) + (typeScore * _typeWeight);
        }

        public async Task UpdateClusterNeighborsAsync()
        {
            _logger.LogInformation("Updating cluster neighbors.");

            var objectives = await _context.Objectives
                .Include(o => o.Reviews)
                .Where(o => o.ClusterId.HasValue && o.Location != null)
                .ToListAsync();

            _context.ClusterNeighbors.RemoveRange(_context.ClusterNeighbors);
            await _context.SaveChangesAsync();

            var neighbors = new List<ClusterNeighbor>();

            foreach (var obj1 in objectives)
            {
                foreach (var obj2 in objectives.Where(o => o.Id != obj1.Id && o.ClusterId == obj1.ClusterId))
                {
                    var distance = obj1.Location!.Distance(obj2.Location!) * 111000;
                    var similarityScore = CalculateSimilarityScore(obj1, obj2);

                    neighbors.Add(new ClusterNeighbor
                    {
                        SourceObjectiveId = obj1.Id,
                        TargetObjectiveId = obj2.Id,
                        ClusterId = obj1.ClusterId!.Value,
                        Distance = distance,
                        SimilarityScore = similarityScore,
                        LastUpdated = DateTime.UtcNow
                    });
                }
            }

            await _context.ClusterNeighbors.AddRangeAsync(neighbors);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cluster neighbors updated: {Count} records.", neighbors.Count);
        }

        public async Task<ServiceResult<List<RecommendedObjectiveDto>>> GetNearestNeighborsAsync(int objectiveId, int count = 4)
        {
            var result = new ServiceResult<List<RecommendedObjectiveDto>>();

            var neighbors = await _context.ClusterNeighbors
                .Include(cn => cn.TargetObjective)
                    .ThenInclude(o => o.Reviews)
                .Include(cn => cn.TargetObjective)
                    .ThenInclude(o => o.Images)
                .Where(cn => cn.SourceObjectiveId == objectiveId)
                .OrderByDescending(cn => cn.SimilarityScore)
                .Take(count)
                .ToListAsync();

            var recommendedObjectives = neighbors.Select(n => new RecommendedObjectiveDto
            {
                Id = n.TargetObjective.Id,
                Name = n.TargetObjective.Name,
                City = n.TargetObjective.City,
                FirstImageUrl = n.TargetObjective.Images.Any()
                    ? ImageHelper.GetFirstImageURL(n.TargetObjective.Images)
                    : null,
                AverageRating = n.TargetObjective.Reviews.Any()
                    ? n.TargetObjective.Reviews.Average(r => r.Raiting)
                    : null,
                Distance = n.Distance
            }).ToList();

            result.Result = recommendedObjectives;
            return result;
        }

        public async Task<ServiceResult<ClusterAnalysisDto>> GetClusterAnalysisAsync()
        {
            return new ServiceResult<ClusterAnalysisDto>();
        }

        private List<SilhouetteScoreDto> CalculateSilhouetteScores(List<Objective> objectives)
            => new List<SilhouetteScoreDto>();

        private List<ClusterStatisticsDto> CalculateClusterStatistics(List<Objective> objectives)
            => new List<ClusterStatisticsDto>();

        private double[,] CalculateClusterSimilarityMatrix(List<Objective> objectives)
        {
            var clusters = objectives.GroupBy(o => o.ClusterId!.Value).ToList();
            return new double[clusters.Count, clusters.Count];
        }

        private double CalculateClusterSimilarity(List<Objective> cluster1, List<Objective> cluster2)
        {
            var totalSimilarity = 0.0;
            var count = 0;
            foreach (var obj1 in cluster1)
            {
                foreach (var obj2 in cluster2)
                {
                    totalSimilarity += CalculateSimilarityScore(obj1, obj2);
                    count++;
                }
            }
            return count > 0 ? totalSimilarity / count : 0;
        }

        private double CalculateAverageDistance(Objective objective, IEnumerable<Objective> otherObjectives)
        {
            if (!otherObjectives.Any()) return 0;
            var totalDistance = 0.0;
            var count = 0;
            foreach (var other in otherObjectives)
            {
                if (objective.Location != null && other.Location != null)
                {
                    totalDistance += objective.Location.Distance(other.Location) * 111000;
                    count++;
                }
            }
            return count > 0 ? totalDistance / count : 0;
        }
    }

    public class RecommendedObjectiveDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? FirstImageUrl { get; set; }
        public double? AverageRating { get; set; }
        public double Distance { get; set; }
    }

    public class ObjectiveVisualizationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double X { get; set; }
        public double Y { get; set; }
        public double DistanceFromCenter { get; set; }
        public int ClusterId { get; set; }
        public double AverageRating { get; set; }
        public double Price { get; set; }
        public string TypeName { get; set; } = string.Empty;
    }

    public class ClusterAnalysisDto
    {
        public List<SilhouetteScoreDto> SilhouetteScores { get; set; } = new();
        public List<ClusterStatisticsDto> ClusterStatistics { get; set; } = new();
        public List<List<double>> SimilarityMatrix { get; set; } = new();
    }

    public class SilhouetteScoreDto
    {
        public int ClusterId { get; set; }
        public double Score { get; set; }
        public int ObjectiveCount { get; set; }
    }

    public class ClusterStatisticsDto
    {
        public int ClusterId { get; set; }
        public double AverageRating { get; set; }
        public double AveragePrice { get; set; }
        public Dictionary<string, int> TypeDistribution { get; set; } = new();
        public int ObjectiveCount { get; set; }
    }
}
