using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelSBE.Entity.Helper;

namespace TravelSBE.Entity
{
    public class Review : BaseAuditEntity
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int IdUser { get; set; }
        public User User { get; set; }
        [ForeignKey("Objective")]
        public int? IdObjective { get; set; }
        public Objective? Objective { get; set; }
        public int Raiting { get; set; }
        public string? Comment { get; set; }
        public DateTime? DatePosted { get; set; }

    }
}
