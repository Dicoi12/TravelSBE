﻿using TravelSBE.Models;

namespace TravelSBE.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserModel> Login(string username, string password);
        Task<bool> SignUp(CreateUser request);
        Task<bool>ChangePassword(int userId, string oldPassword, string newPassword);
    }
}
