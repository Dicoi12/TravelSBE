using Microsoft.AspNetCore.Mvc;
using TravelSBE.Models;
using TravelSBE.Services.Interfaces;

namespace TravelSBE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<bool> Login(string userName, string password)
        {
            return await _userService.Login(userName, password);
        }
        [HttpPost]
        public async Task<bool> SignUp(CreateUser request)
        {
            return await _userService.SignUp(request);
        }
    }
}
