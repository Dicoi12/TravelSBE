using System.ComponentModel.DataAnnotations;

namespace TravelSBE.Dtos
{
    public class ChangePasswordDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string OldPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
