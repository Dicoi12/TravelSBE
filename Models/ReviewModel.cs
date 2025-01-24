using System.ComponentModel.DataAnnotations.Schema;
using TravelSBE.Entity;
using TravelSBE.Entity.Helper;

namespace TravelSBE.Models
{
    public class ReviewModel : BaseAuditEntity
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public User? User { get; set; }
        public int? IdObjective { get; set; }
        public Objective? Objective { get; set; }
        public int Raiting { get; set; }
        public string? Comment { get; set; }
        public DateTime? DatePosted { get; set; }
    }
}
