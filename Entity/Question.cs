using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelSBE.Entity.Helper;

namespace TravelSBE.Entity
{
    public class Question : BaseAuditEntity
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int IdUser { get; set; }
        public User User { get; set; }
        [ForeignKey("Objective")]
        public int? IdObjective { get; set; }
        public Objective? Objective { get; set; }
        [ForeignKey("Event")]
        public int? IdEvent { get; set; }
        public Event? Event { get; set; }
        public string Text { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.Now;

    }
}
