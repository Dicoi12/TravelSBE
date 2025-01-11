using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelSBE.Entity;
using TravelSBE.Entity.Helper;
using static System.Net.Mime.MediaTypeNames;

namespace TravelsBE.Entity
{
    public class Experience : BaseAuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(100)]
        public string? Name { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }
        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        public string? LocationName { get; set; }

        public int? Rating { get; set; }

        public List<ObjectiveImage> Images { get; set; }
        public bool IsPublic { get; set; } = true;

    }
}
