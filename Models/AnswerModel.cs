using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelSBE.Entity;
using TravelSBE.Entity.Helper;

namespace TravelSBE.Models
{
    public class AnswerModel : BaseAuditEntity
    {
        public int Id { get; set; }
        public int IdQuestion { get; set; }
        public Question Question { get; set; }
        public int IdUser { get; set; }
        public User User { get; set; }
        public string Text { get; set; }
        public DateTime DatePosted { get; set; }
    }
}
