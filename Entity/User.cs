using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelSBE.Entity.Helper;
using TravelSBE.Enums;

namespace TravelSBE.Entity
{
    public class User : BaseAuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public UserRoleEnum Role { get; set; }
        public string Hash {  get; set; }
        public string Salt { get; set; }
    }
}
