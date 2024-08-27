using AutoMapper;
using TravelSBE.Data;
using TravelSBE.Entity;
using TravelSBE.Models;
using TravelSBE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TravelSBE.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> SignUp(CreateUser request)
        {
            if (await _context.Users.AnyAsync(u => u.UserName == request.UserName))
            {
                return false; 
            }

            string salt = PasswordHelper.CreateSalt();
            string hash = PasswordHelper.HashPassword(request.Password, salt);

            var user = _mapper.Map<User>(request);
            user.Salt = salt;
            user.Hash = hash;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return false;
            }

            bool isPasswordValid = PasswordHelper.VerifyPassword(password, user.Hash, user.Salt);
            return isPasswordValid;
        }
    }
}
