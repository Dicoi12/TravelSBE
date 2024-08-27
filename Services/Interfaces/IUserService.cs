using TravelSBE.Models;

namespace TravelSBE.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> Login(string username, string password);
        Task<bool> SignUp(CreateUser request);
    }
}
