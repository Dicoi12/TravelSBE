using System.ComponentModel.DataAnnotations.Schema;
using TravelSBE.Entity;
using TravelSBE.Entity.Helper;

namespace TravelSBE.Models
{
    public class ObjectiveImageModel : BaseAuditEntity
    {
        public int Id { get; set; }

        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
        public string ImageDataBase64 { get; set; }
        public int? IdObjective { get; set; }
        public Objective? Objective { get; set; }
    }
}
