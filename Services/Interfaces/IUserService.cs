using TravelsBE.Dtos;
using TravelSBE.Dtos;
using TravelSBE.Models;

namespace TravelSBE.Services.Interfaces
{
    public interface IUserService
    {
        Task<TokenResponse> Login(string username, string password);
        Task<bool> SignUp(CreateUser request);
        Task<bool> ChangePassword(ChangePasswordDto change);

        Task<UserModel> GetUserData();
    }
}
