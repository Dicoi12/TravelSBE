using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelSBE.Entity.Helper;

namespace TravelSBE.Entity
{
    public class Answer : BaseAuditEntity
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Question")]
        public int IdQuestion { get; set; }
        public Question Question { get; set; }
        [ForeignKey("User")]
        public int IdUser { get; set; }
        public User User { get; set; }
        public string Text { get; set; }
        public DateTime DatePosted { get; set; }
    }
}
