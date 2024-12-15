using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TravelSBE.Entity;
using TravelsBE.Enums;

namespace TravelsBE.Entity
{
    public class ObjectiveSchedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 

        [ForeignKey("Objective")]
        public int ObjectiveId { get; set; } 

        public DayOfWeekEnum DayOfWeek { get; set; } 

        public TimeSpan OpenTime { get; set; } 

        public TimeSpan CloseTime { get; set; } 

        public decimal EntryPrice { get; set; } 
        public virtual Objective Objective { get; set; }
    }
}
