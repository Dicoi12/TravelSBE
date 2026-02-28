namespace TravelSBE.Models
{
    public class CreateUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; } = string.Empty;
    }
}
