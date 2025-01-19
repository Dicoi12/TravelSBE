using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using TravelsBE.Entity;
using TravelSBE.Entity.Helper;

namespace TravelSBE.Entity
{
    public class Objective : BaseAuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? City { get; set; }
        [ForeignKey(nameof(ObjectiveType))]
        public int? Type { get; set; }
        public ObjectiveType? ObjectiveType { get; set; }
        public string? Website { get; set; }

        public List<ObjectiveImage> Images { get; set; } = new List<ObjectiveImage>();
        public List<Review> Reviews { get; set; } = new List<Review>();


    }
}
