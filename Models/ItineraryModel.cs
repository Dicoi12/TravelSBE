using System.ComponentModel.DataAnnotations.Schema;
using TravelSBE.Entity;
using TravelSBE.Entity.Helper;

namespace TravelSBE.Models
{
    public class ItineraryModel : BaseAuditEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? IdUser { get; set; }
        public User? User { get; set; }
        public List<ObjectiveImage> Images { get; set; } = new List<ObjectiveImage>();
        public DateTime? DataStart { get; set; }
        public DateTime? DataStop { get; set; }
    }
}
