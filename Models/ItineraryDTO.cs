namespace TravelsBE.Models
{
    public class ItineraryDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> ObjectivesIds { get; set; } = new List<int>();
        public int? IdUser { get; set; }
        public List<int> EventsIds { get; set; } = new List<int>();
    }
}
