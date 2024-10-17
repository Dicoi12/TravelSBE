using System.ComponentModel.DataAnnotations;
using TravelSBE.Entity.Helper;
using TravelSBE.Enums;

namespace TravelSBE.Models
{
    public class UserModel : BaseAuditEntity
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public UserRoleEnum Role { get; set; }
    }
}
