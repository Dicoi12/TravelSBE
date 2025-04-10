using System.ComponentModel.DataAnnotations.Schema;
using TravelSBE.Entity;
using TravelSBE.Entity.Helper;

namespace TravelSBE.Models
{
    public class ItineraryDetailModel : BaseAuditEntity
    {
        public int Id { get; set; }
        public int? IdObjective { get; set; }
        public ObjectiveModel? Objective { get; set; }
        public int? IdEvent { get; set; }
        public EventModel? Event { get; set; }
        public int VisitOrder { get; set; }
        public int IdItinerary { get; set; }
        public string[] Images { get; set; } = new string[0];
        public string Name { get; set; }
        public string? Descriere { get; set; }
    }
}
