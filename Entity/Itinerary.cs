using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelSBE.Entity.Helper;

namespace TravelSBE.Entity
{
    public class Itinerary : BaseAuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("User")]
        public int IdUser { get; set; }
        public User User { get; set; }
    }
}
