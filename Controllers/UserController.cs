using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelSBE.Dtos;
using TravelSBE.Models;
using TravelSBE.Services.Interfaces;

namespace TravelSBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<TokenResponse> Login([FromBody] LoginDto loginDto)
        {
            return await _userService.Login(loginDto.UserName, loginDto.Password);
        }

        [AllowAnonymous]
        [HttpPost("SignUp")]
        public async Task<bool> SignUp([FromBody] CreateUser request)
        {
            return await _userService.SignUp(request);
        }

        [HttpPost("ChangePassword")]
        public async Task<bool> ChangePassword([FromBody] ChangePasswordDto request)
        {
            return await _userService.ChangePassword(request.UserId, request.OldPassword, request.NewPassword);
        }
    }
}
