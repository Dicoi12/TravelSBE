using TravelSBE.Entity;
using TravelSBE.Models;

namespace TravelsBE.Models
{
    public class ItineraryPageDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? IdUser { get; set; }
        public ItineraryDetailModel[] ItineraryDetails { get; set; }
    }
}
