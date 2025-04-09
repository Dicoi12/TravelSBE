using TravelSBE.Entity;

namespace TravelsBE.Models
{
    public class ItineraryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? IdUser { get; set; } = null;
        public ItineraryDetail[] ItineraryDetails { get; set; } 
    }
}
