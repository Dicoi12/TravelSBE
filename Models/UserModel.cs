using System.ComponentModel.DataAnnotations;
using TravelSBE.Entity.Helper;

namespace TravelSBE.Models
{
    public class UserModel : BaseAuditEntity
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
    }
}
