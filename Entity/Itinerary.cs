using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelSBE.Entity.Helper;

namespace TravelSBE.Entity
{
    public class Itinerary : BaseAuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;

        [ForeignKey("IdUser")]
        public int? IdUser { get; set; }
        public User? User { get; set; }

        // Lista cu detaliile itinerariului
        public List<ItineraryDetail> ItineraryDetails { get; set; } = new List<ItineraryDetail>();
    }
}
