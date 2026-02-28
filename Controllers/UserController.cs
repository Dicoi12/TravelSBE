using Microsoft.AspNetCore.Mvc;
using TravelsBE.Dtos;
using TravelSBE.Dtos;
using TravelSBE.Models;
using TravelSBE.Services.Interfaces;

namespace TravelSBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Login")]
        public async Task<TokenResponse> Login([FromBody] LoginDto loginDto)
        {
            return await _userService.Login(loginDto.UserName, loginDto.Password);
        }

        [HttpPost("SignUp")]
        public async Task<bool> SignUp(CreateUser request)
        {
            return await _userService.SignUp(request);
        }
        [HttpPost("ChangePassword")]
        public async Task<bool> ChangePassword(int userId, string oldPassword, string newPassword)
        {
            return await _userService.ChangePassword(userId, oldPassword, newPassword);
        }
    }
}
