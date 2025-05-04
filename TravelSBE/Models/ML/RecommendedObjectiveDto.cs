namespace TravelSBE.Models.ML
{
    public class RecommendedObjectiveDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string? FirstImageUrl { get; set; }
        public double? AverageRating { get; set; }
    }
} 