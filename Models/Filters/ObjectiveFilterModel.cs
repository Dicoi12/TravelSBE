namespace TravelsBE.Models.Filters
{
    public class ObjectiveFilterModel
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? MaxDistance { get; set; } // Distanță maximă în km
        public string? Name { get; set; } // Numele obiectivului
        public int? TypeId { get; set; } // ID-ul tipului de obiectiv
        public double? MinRating { get; set; } // Rating minim
    }
}
