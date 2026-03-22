using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelSBE.Entity;
using TravelSBE.Entity.Helper;

namespace TravelSBE.Models
{
    public class ReviewModel : BaseAuditEntity
    {
        public int Id { get; set; }

        [Required]
        public int IdUser { get; set; }

        public User? User { get; set; }

        [Required]
        public int? IdObjective { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Raiting { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }

        public DateTime? DatePosted { get; set; }
    }
}
