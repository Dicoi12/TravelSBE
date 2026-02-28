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
        Task UpdateClusterNeighborsAsync();
        Task<ServiceResult<List<RecommendedObjectiveDto>>> GetNearestNeighborsAsync(int objectiveId, int count = 4);
        Task<ServiceResult<ClusterAnalysisDto>> GetClusterAnalysisAsync();
    }

    public class MLService : IMLService
    {
        private readonly ApplicationDbContext _context;
        private const int K = 5;
        private const int MAX_ITERATIONS = 100;
        private readonly Point _referencePoint;

        private class ClusteringPoint
        {
            public Objective Objective { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public double AverageRating { get; set; }
            public double Price { get; set; }
            public int TypeId { get; set; }
        }

        public MLService(ApplicationDbContext context)
        {
            _context = context;
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
                    Y = o.Location?.Y ?? 0,
                    AverageRating = o.Reviews.Any() ? o.Reviews.Average(r => r.Raiting) : 0,
                    Price = !string.IsNullOrEmpty(o.Pret) ? Int32.Parse(o.Pret) : 0,
                    TypeId = o.Type ?? 0
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

        private async Task<List<(double x, double y, double rating, double price, double type)>> NormalizePoints(List<ClusteringPoint> points)
        {
            var normalizedPoints = new List<(double x, double y, double rating, double price, double type)>();

            var xValues = points.Select(p => p.X).ToList();
            var yValues = points.Select(p => p.Y).ToList();
            var ratingValues = points.Select(p => p.AverageRating).ToList();
            var priceValues = points.Select(p => p.Price).ToList();
            var typeValues = points.Select(p => (double)p.TypeId).ToList();

            var minX = xValues.Min();
            var maxX = xValues.Max();
            var minY = yValues.Min();
            var maxY = yValues.Max();
            var minRating = ratingValues.Min();
            var maxRating = ratingValues.Max();
            var minPrice = priceValues.Min();
            var maxPrice = priceValues.Max();
            var minType = typeValues.Min();
            var maxType = typeValues.Max();

            // verific max min
            if (maxX == minX) maxX = minX + 1;
            if (maxY == minY) maxY = minY + 1;
            if (maxRating == minRating) maxRating = minRating + 1;
            if (maxPrice == minPrice) maxPrice = minPrice + 1;
            if (maxType == minType) maxType = minType + 1;

            // ponderi
            const double locationWeight = 0.6;   
            const double ratingWeight = 0.1;     
            const double priceWeight = 0.1;       
            const double typeWeight = 0.2;        

            foreach (var point in points)
            {
                var normalizedX = ((point.X - minX) / (maxX - minX)) * locationWeight;
                var normalizedY = ((point.Y - minY) / (maxY - minY)) * locationWeight;
                var normalizedRating = ((point.AverageRating - minRating) / (maxRating - minRating)) * ratingWeight;
                var normalizedPrice = ((point.Price - minPrice) / (maxPrice - minPrice)) * priceWeight;
                var normalizedType = ((point.TypeId - minType) / (maxType - minType)) * typeWeight;

                normalizedPoints.Add((normalizedX, normalizedY, normalizedRating, normalizedPrice, normalizedType));
            }

            return normalizedPoints;
        }

        private List<int> ApplyKMeans(List<(double x, double y, double rating, double price, double type)> points)
        {
            var centroids = new List<(double x, double y, double rating, double price, double type)>();
            var random = new Random();

            for (int i = 0; i < K; i++)
            {
                centroids.Add(points[random.Next(points.Count)]);
            }

            var clusters = new List<int>();
            var oldClusters = new List<int>();
            var iterations = 0;

            do
            {
                oldClusters = clusters;
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

                // Actualizează centroizii
                for (int i = 0; i < K; i++)
                {
                    var clusterPoints = points.Where((p, index) => clusters[index] == i).ToList();
                    if (clusterPoints.Any())
                    {
                        var newX = clusterPoints.Average(p => p.x);
                        var newY = clusterPoints.Average(p => p.y);
                        var newRating = clusterPoints.Average(p => p.rating);
                        var newPrice = clusterPoints.Average(p => p.price);
                        var newType = clusterPoints.Average(p => p.type);
                        centroids[i] = (newX, newY, newRating, newPrice, newType);
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
        //euclidiana
        private double CalculateDistance((double x, double y, double rating, double price, double type) p1, (double x, double y, double rating, double price, double type) p2)
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

            var objective = await _context.Objectives
                .FirstOrDefaultAsync(o => o.Id == objectiveId);

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
                    Distance = o.Location.Distance(objective.Location) * 111000
                })
                .OrderBy(x => x.Distance)
                .Take(count)
                .Select(x => new RecommendedObjectiveDto
                {
                    Id = x.Objective.Id,
                    Name = x.Objective.Name,
                    City = x.Objective.City,
                    FirstImageUrl = x.Objective.Images.Any() ?
                        ImageHelper.GetFirstImageURL(x.Objective.Images) :
                        null,
                    AverageRating = x.Objective.Reviews.Any() ?
                        x.Objective.Reviews.Average(r => r.Raiting) :
                        null,
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

                Price = !string.IsNullOrEmpty(o.Pret) ? Int32.Parse(o.Pret) : 0,
                TypeName = o.ObjectiveType?.Name ?? "Necunoscut"
            }).ToList();

            result.Result = visualizationData;
            return result;
        }

        private double CalculateSimilarityScore(Objective obj1, Objective obj2)
        {
            var locationScore = 1.0 - (obj1.Location.Distance(obj2.Location) * 111000 / 100000); // Normalizat la 100km
            var ratingScore = obj1.Reviews.Any() && obj2.Reviews.Any() 
                ? 1.0 - Math.Abs(obj1.Reviews.Average(r => r.Raiting) - obj2.Reviews.Average(r => r.Raiting)) / 5.0 
                : 0.5;
            var priceScore = !string.IsNullOrEmpty(obj1.Pret) && !string.IsNullOrEmpty(obj2.Pret)
                ? 1.0 - Math.Abs(double.Parse(obj1.Pret) - double.Parse(obj2.Pret)) / 1000.0
                : 0.5;
            var typeScore = obj1.Type == obj2.Type ? 1.0 : 0.0;

            return (locationScore * 0.6) + (ratingScore * 0.1) + (priceScore * 0.1) + (typeScore * 0.2);
        }

        public async Task UpdateClusterNeighborsAsync()
        {
            var objectives = await _context.Objectives
                .Include(o => o.Reviews)
                .Where(o => o.ClusterId.HasValue && o.Location != null)
                .ToListAsync();

            // Șterge vechile relații
            _context.ClusterNeighbors.RemoveRange(_context.ClusterNeighbors);
            await _context.SaveChangesAsync();

            var neighbors = new List<ClusterNeighbor>();

            foreach (var obj1 in objectives)
            {
                foreach (var obj2 in objectives.Where(o => o.Id != obj1.Id && o.ClusterId == obj1.ClusterId))
                {
                    var distance = obj1.Location.Distance(obj2.Location) * 111000; // în metri
                    var similarityScore = CalculateSimilarityScore(obj1, obj2);

                    neighbors.Add(new ClusterNeighbor
                    {
                        SourceObjectiveId = obj1.Id,
                        TargetObjectiveId = obj2.Id,
                        ClusterId = obj1.ClusterId.Value,
                        Distance = distance,
                        SimilarityScore = similarityScore,
                        LastUpdated = DateTime.UtcNow
                    });
                }
            }

            await _context.ClusterNeighbors.AddRangeAsync(neighbors);
            await _context.SaveChangesAsync();
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
            var result = new ServiceResult<ClusterAnalysisDto>();
            
            //var objectives = await _context.Objectives
            //    .Include(o => o.Reviews)
            //    .Include(o => o.ObjectiveType)
            //    .Where(o => o.ClusterId.HasValue)
            //    .ToListAsync();

            //var silhouetteScores = CalculateSilhouetteScores(objectives);
            //var clusterStats = CalculateClusterStatistics(objectives);
            //var similarityMatrix = CalculateClusterSimilarityMatrix(objectives);

            //// Convertim matricea 2D într-o listă de liste pentru serializare
            //var serializableMatrix = new List<List<double>>();
            //for (int i = 0; i < similarityMatrix.GetLength(0); i++)
            //{
            //    var row = new List<double>();
            //    for (int j = 0; j < similarityMatrix.GetLength(1); j++)
            //    {
            //        row.Add(similarityMatrix[i, j]);
            //    }
            //    serializableMatrix.Add(row);
            //}

            //result.Result = new ClusterAnalysisDto
            //{
            //    SilhouetteScores = silhouetteScores,
            //    ClusterStatistics = clusterStats,
            //    SimilarityMatrix = serializableMatrix
            //};

            return result;
        }

        private List<SilhouetteScoreDto> CalculateSilhouetteScores(List<Objective> objectives)
        {
            var scores = new List<SilhouetteScoreDto>();
            //var clusters = objectives.GroupBy(o => o.ClusterId.Value).ToList();

            //foreach (var cluster in clusters)
            //{
            //    var clusterObjectives = cluster.ToList();
            //    var clusterScore = 0.0;

            //    foreach (var objective in clusterObjectives)
            //    {
            //        var a = CalculateAverageDistance(objective, clusterObjectives.Where(o => o.Id != objective.Id));
            //        var b = double.MaxValue;

            //        foreach (var otherCluster in clusters.Where(c => c.Key != cluster.Key))
            //        {
            //            var distance = CalculateAverageDistance(objective, otherCluster);
            //            b = Math.Min(b, distance);
            //        }

            //        var score = (b - a) / Math.Max(a, b);
            //        clusterScore += score;
            //    }

            //    scores.Add(new SilhouetteScoreDto
            //    {
            //        ClusterId = cluster.Key,
            //        Score = clusterScore / clusterObjectives.Count,
            //        ObjectiveCount = clusterObjectives.Count
            //    });
            //}

            return scores;
        }

        private List<ClusterStatisticsDto> CalculateClusterStatistics(List<Objective> objectives)
        {
            var stats = new List<ClusterStatisticsDto>();
            //var clusters = objectives.GroupBy(o => o.ClusterId.Value);

            //foreach (var cluster in clusters)
            //{
            //    var clusterObjectives = cluster.ToList();
            //    var avgRating = clusterObjectives.Average(o => o.Reviews.Any() ? o.Reviews.Average(r => r.Raiting) : 0);
            //    var avgPrice = clusterObjectives.Average(o => !string.IsNullOrEmpty(o.Pret) ? double.Parse(o.Pret) : 0);
            //    var typeDistribution = clusterObjectives
            //        .GroupBy(o => o.ObjectiveType?.Name ?? "Necunoscut")
            //        .ToDictionary(g => g.Key, g => g.Count());

            //    stats.Add(new ClusterStatisticsDto
            //    {
            //        ClusterId = cluster.Key,
            //        AverageRating = avgRating,
            //        AveragePrice = avgPrice,
            //        TypeDistribution = typeDistribution,
            //        ObjectiveCount = clusterObjectives.Count
            //    });
            //}

            return stats;
        }

        private double[,] CalculateClusterSimilarityMatrix(List<Objective> objectives)
        {
            var clusters = objectives.GroupBy(o => o.ClusterId.Value).ToList();
            var matrix = new double[clusters.Count, clusters.Count];

            //for (int i = 0; i < clusters.Count; i++)
            //{
            //    for (int j = 0; j < clusters.Count; j++)
            //    {
            //        if (i == j)
            //        {
            //            matrix[i, j] = 1.0;
            //            continue;
            //        }

            //        var similarity = CalculateClusterSimilarity(clusters[i].ToList(), clusters[j].ToList());
            //        matrix[i, j] = similarity;
            //    }
            //}

            return matrix;
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
                    totalDistance += objective.Location.Distance(other.Location) * 111000; // în metri
                    count++;
                }
            }

            return count > 0 ? totalDistance / count : 0;
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
    }

    public class ObjectiveVisualizationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double DistanceFromCenter { get; set; }
        public int ClusterId { get; set; }
        public double AverageRating { get; set; }
        public double Price { get; set; }
        public string TypeName { get; set; }
    }

    public class ClusterAnalysisDto
    {
        public List<SilhouetteScoreDto> SilhouetteScores { get; set; }
        public List<ClusterStatisticsDto> ClusterStatistics { get; set; }
        public List<List<double>> SimilarityMatrix { get; set; }
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
        public Dictionary<string, int> TypeDistribution { get; set; }
        public int ObjectiveCount { get; set; }
    }
}