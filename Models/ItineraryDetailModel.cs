using System.ComponentModel.DataAnnotations.Schema;
using TravelSBE.Entity;
using TravelSBE.Entity.Helper;

namespace TravelSBE.Models
{
    public class ItineraryDetailModel:BaseAuditEntity
    {
        public int Id { get; set; }
        public int? IdObjective { get; set; }
        public Objective? Objective { get; set; }
        public int? IdEvent { get; set; }
        public Event? Event { get; set; }
        public int VisitOrder { get; set; }
    }
}
