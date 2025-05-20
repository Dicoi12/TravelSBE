using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSBE.Entity
{
    public class ClusterNeighbor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SourceObjectiveId { get; set; }

        [Required]
        public int TargetObjectiveId { get; set; }

        [Required]
        public int ClusterId { get; set; }

        [Required]
        public double Distance { get; set; }

        [Required]
        public double SimilarityScore { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }

        [ForeignKey("SourceObjectiveId")]
        public virtual Objective SourceObjective { get; set; }

        [ForeignKey("TargetObjectiveId")]
        public virtual Objective TargetObjective { get; set; }
    }
} 