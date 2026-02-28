using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TravelsBE.Dtos;
using TravelSBE.Data;
using TravelSBE.Entity;
using TravelSBE.Models;
using TravelSBE.Services.Interfaces;

namespace TravelSBE.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;


    public UserService(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _config = configuration;
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

    public async Task<TokenResponse> Login(string username, string password)
    {
        TokenResponse response = new TokenResponse();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (user == null)
        {
            return response;
        }

        bool isPasswordValid = PasswordHelper.VerifyPassword(password, user.Hash, user.Salt);
        if (isPasswordValid)
        {
            return GenerateJwtToken(user);
        }
        else
        {
            return response;
        }
    }

    public async Task<bool> ChangePassword(int userId, string currentPassword, string newPassword)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
        {
            return false;
        }
        bool isPasswordValid = PasswordHelper.VerifyPassword(currentPassword, user.Hash, user.Salt);
        if (isPasswordValid)
        {
            string salt = PasswordHelper.CreateSalt();
            string hash = PasswordHelper.HashPassword(newPassword, salt);
            user.Salt = salt;
            user.Hash = hash;
            _context.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }


    private TokenResponse GenerateJwtToken(User user)
    {
        TokenResponse response = new TokenResponse();
        var jwtSettings = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

        var claims = new[]
        {
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(ClaimTypes.Email, user.Email),
    new Claim(ClaimTypes.Name, user.UserName)
};

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),
            signingCredentials: creds
        );

        response.Token = new JwtSecurityTokenHandler().WriteToken(token);
        return response;
    }

}

