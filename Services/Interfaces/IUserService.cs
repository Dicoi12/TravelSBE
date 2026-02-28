using TravelSBE.Models;

using TravelsBE.Dtos;

namespace TravelSBE.Services.Interfaces
{
    public interface IUserService
    {
        Task<TokenResponse> Login(string username, string password);
        Task<bool> SignUp(CreateUser request);
        Task<bool> ChangePassword(int userId, string oldPassword, string newPassword);
    }
}
