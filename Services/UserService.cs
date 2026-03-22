using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TravelsBE.Dtos;
using TravelsBE.Services.Interfaces;
using TravelSBE.Data;
using TravelSBE.Dtos;
using TravelSBE.Entity;
using TravelSBE.Models;
using TravelSBE.Services.Interfaces;

namespace TravelSBE.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    private readonly ILogger<UserService> _logger;
    private readonly ICurrentUserService _currentUserService;

    public UserService(ApplicationDbContext context,ICurrentUserService currentUserService, IMapper mapper, IConfiguration configuration, ILogger<UserService> logger)
    {
        _context = context;
        _mapper = mapper;
        _config = configuration;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<bool> SignUp(CreateUser request)
    {
        if (await _context.Users.AnyAsync(u => u.UserName == request.UserName))
        {
            _logger.LogWarning("Sign-up failed: username '{UserName}' already exists.", request.UserName);
            return false;
        }

        string salt = PasswordHelper.CreateSalt();
        string hash = PasswordHelper.HashPassword(request.Password, salt);

        var user = new User
        {
            Email = request.Email,
            UserName = request.UserName,
            Salt = salt,
            Hash = hash
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User '{UserName}' registered successfully.", request.UserName);
        return true;
    }

    public async Task<TokenResponse> Login(string username, string password)
    {
        var response = new TokenResponse();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (user == null)
        {
            _logger.LogWarning("Login failed: user '{UserName}' not found.", username);
            return response;
        }

        bool isPasswordValid = PasswordHelper.VerifyPassword(password, user.Hash, user.Salt);
        if (isPasswordValid)
        {
            _logger.LogInformation("User '{UserName}' logged in.", username);
            return GenerateJwtToken(user);
        }

        _logger.LogWarning("Login failed: invalid password for user '{UserName}'.", username);
        return response;
    }

    public async Task<bool> ChangePassword(ChangePasswordDto dto)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return false;

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId.Value);
        if (user == null)
        {
            _logger.LogWarning("ChangePassword failed: user {UserId} not found.", userId);
            return false;
        }

        bool isPasswordValid = PasswordHelper.VerifyPassword(dto.OldPassword, user.Hash, user.Salt);
        if (!isPasswordValid)
        {
            _logger.LogWarning("ChangePassword failed: invalid current password for user {UserId}.", userId);
            return false;
        }

        string salt = PasswordHelper.CreateSalt();
        string hash = PasswordHelper.HashPassword(dto.NewPassword, salt);
        user.Salt = salt;
        user.Hash = hash;
        _context.Update(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Password changed for user {UserId}.", userId);
        return true;
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

    public async Task<UserModel> GetUserData()
    {
        UserModel result = new UserModel();
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == _currentUserService.GetUserIdOrNull());
        result = _mapper.Map<UserModel>(user);
        return result;
    }
}

