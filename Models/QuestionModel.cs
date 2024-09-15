using System.ComponentModel.DataAnnotations.Schema;
using TravelSBE.Entity;
using TravelSBE.Entity.Helper;

namespace TravelSBE.Models
{
    public class QuestionModel : BaseAuditEntity
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public User User { get; set; }
        public int? IdObjective { get; set; }
        public Objective? Objective { get; set; }
        public int? IdEvent { get; set; }
        public Event? Event { get; set; }
        public string Text { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.Now;
    }
}
