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

            User user = new User
            {
                Email = request.Email,
                UserName = request.UserName,
            };
            user.Salt = salt;
            user.Hash = hash;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<UserModel> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return new UserModel();
            }

            bool isPasswordValid = PasswordHelper.VerifyPassword(password, user.Hash, user.Salt);
            if (isPasswordValid)
            {
                var mapped = _mapper.Map<UserModel>(user);
                return mapped;
            }
            else
            {
                return new UserModel();
            }
        }

    }
}
