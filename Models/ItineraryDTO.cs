using System.ComponentModel.DataAnnotations;
using TravelSBE.Entity;

namespace TravelsBE.Models
{
    public class ItineraryDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        public int? IdUser { get; set; } = null;

        public ItineraryDetail[] ItineraryDetails { get; set; } = Array.Empty<ItineraryDetail>();

        public DateTime? DataStart { get; set; }
        public DateTime? DataStop { get; set; }
    }
}
