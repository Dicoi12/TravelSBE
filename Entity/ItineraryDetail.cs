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
        [Required]
        public string Name { get; set; }
        public string? Descriere {  get; set; }
        [ForeignKey("IdItinerary")]
        public int IdItinerary { get; set; }
        public Itinerary Itinerary { get; set; }
        [ForeignKey("IdObjective")]
        public int? IdObjective { get; set; }
        public Objective? Objective { get; set; }
        [ForeignKey("IdEvent")]
        public int? IdEvent { get; set; }
        public Event? Event { get; set; }
        public int VisitOrder { get; set; }
    }
}
