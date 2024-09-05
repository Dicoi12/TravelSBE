using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelSBE.Entity.Helper;

namespace TravelSBE.Entity
{
    public class ItineraryDetail : BaseAuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }
        [ForeignKey("Objective")]
        public int? IdObjective { get; set; }
        public Objective? Objective { get; set; }
        [ForeignKey("Event")]
        public int? IdEvent { get; set; }
        public Event? Event { get; set; }
        public int VisitOrder { get; set; }
    }
}
