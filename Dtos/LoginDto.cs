using System.ComponentModel.DataAnnotations;

namespace TravelSBE.Dtos
{
    public class LoginDto
    {
        [Required]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
