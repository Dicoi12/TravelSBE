 using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelSBE.Entity.Helper;

namespace TravelSBE.Entity
{
    public class ObjectiveImage : BaseAuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public byte[] ImageData { get; set; }
        public string FilePath { get; set; } // Calea relativă a imaginii în sistemul local
        public string ImageMimeType { get; set; }

        [ForeignKey("IdObjective")]
        public int? IdObjective { get; set; }
        public Objective? Objective { get; set; }
        [ForeignKey("IdEvent")]
        public int? IdEvent { get; set; }
        public Event? Event { get; set; }
    }
}
