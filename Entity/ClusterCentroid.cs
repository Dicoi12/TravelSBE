using System.ComponentModel.DataAnnotations;

namespace TravelSBE.Entity
{
    public class ClusterCentroid
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClusterId { get; set; }

        [Required]
        public double CentroidX { get; set; }

        [Required]
        public double CentroidY { get; set; }

        [Required]
        public double CentroidRating { get; set; }

        [Required]
        public double CentroidPrice { get; set; }

        [Required]
        public double CentroidType { get; set; }

        public int ObjectiveCount { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }
    }
}
