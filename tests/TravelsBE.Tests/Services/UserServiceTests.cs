using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TravelsBE.Services.Interfaces;
using TravelSBE.Data;
using TravelSBE.Dtos;
using TravelSBE.Entity;
using TravelSBE.Mapper;
using TravelSBE.Models;
using TravelSBE.Services;

namespace TravelsBE.Tests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task SignUp_PersistsUserWithHashedPassword()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var created = await service.SignUp(new CreateUser
        {
            UserName = "ana",
            Email = "ana@example.com",
            Password = "Parola123!"
        });

        var user = await context.Users.SingleAsync();

        Assert.True(created);
        Assert.Equal("ana", user.UserName);
        Assert.NotEqual("Parola123!", user.Hash);
        Assert.NotEmpty(user.Salt);
        Assert.True(PasswordHelper.VerifyPassword("Parola123!", user.Hash, user.Salt));
    }

    [Fact]
    public async Task SignUp_ReturnsFalse_WhenUsernameAlreadyExists()
    {
        await using var context = CreateContext();
        await SeedUser(context, "ana", "Parola123!");
        var service = CreateService(context);

        var created = await service.SignUp(new CreateUser
        {
            UserName = "ana",
            Email = "alta@example.com",
            Password = "AltaParola123!"
        });

        Assert.False(created);
        Assert.Equal(1, await context.Users.CountAsync());
    }

    [Fact]
    public async Task Login_ReturnsJwtToken_WhenCredentialsAreValid()
    {
        await using var context = CreateContext();
        var user = await SeedUser(context, "ana", "Parola123!", "ana@example.com");
        var service = CreateService(context);

        var response = await service.Login("ana", "Parola123!");

        Assert.False(string.IsNullOrWhiteSpace(response.Token));

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(response.Token);
        Assert.Equal(user.Id.ToString(), jwt.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);
        Assert.Equal("ana@example.com", jwt.Claims.Single(c => c.Type == ClaimTypes.Email).Value);
        Assert.Equal("ana", jwt.Claims.Single(c => c.Type == ClaimTypes.Name).Value);
    }

    [Fact]
    public async Task Login_ReturnsEmptyResponse_WhenPasswordIsInvalid()
    {
        await using var context = CreateContext();
        await SeedUser(context, "ana", "Parola123!");
        var service = CreateService(context);

        var response = await service.Login("ana", "ParolaGresita123!");

        Assert.Null(response.Token);
    }

    [Fact]
    public async Task ChangePassword_UpdatesHash_WhenOldPasswordIsValid()
    {
        await using var context = CreateContext();
        var user = await SeedUser(context, "ana", "Parola123!");
        var service = CreateService(context, currentUserId: user.Id);
        var oldHash = user.Hash;
        var oldSalt = user.Salt;

        var changed = await service.ChangePassword(new ChangePasswordDto
        {
            OldPassword = "Parola123!",
            NewPassword = "ParolaNoua123!"
        });

        var updatedUser = await context.Users.SingleAsync();

        Assert.True(changed);
        Assert.NotEqual(oldHash, updatedUser.Hash);
        Assert.NotEqual(oldSalt, updatedUser.Salt);
        Assert.True(PasswordHelper.VerifyPassword("ParolaNoua123!", updatedUser.Hash, updatedUser.Salt));
        Assert.False(PasswordHelper.VerifyPassword("Parola123!", updatedUser.Hash, updatedUser.Salt));
    }

    [Fact]
    public async Task ChangePassword_ReturnsFalse_WhenNoCurrentUserExists()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var changed = await service.ChangePassword(new ChangePasswordDto
        {
            OldPassword = "Parola123!",
            NewPassword = "ParolaNoua123!"
        });

        Assert.False(changed);
    }

    private static UserService CreateService(ApplicationDbContext context, int? currentUserId = null)
    {
        return new UserService(
            context,
            new TestCurrentUserService(currentUserId),
            CreateMapper(),
            CreateConfiguration(),
            NullLogger<UserService>.Instance);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private static IConfiguration CreateConfiguration()
    {
        var values = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "test-secret-key-with-at-least-32-chars",
            ["Jwt:Issuer"] = "TravelsBE.Tests",
            ["Jwt:Audience"] = "TravelsBE.Client",
            ["Jwt:ExpireMinutes"] = "60"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }

    private static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        return configuration.CreateMapper();
    }

    private static async Task<User> SeedUser(
        ApplicationDbContext context,
        string username,
        string password,
        string email = "ana@example.com")
    {
        var salt = PasswordHelper.CreateSalt();
        var user = new User
        {
            UserName = username,
            Email = email,
            Salt = salt,
            Hash = PasswordHelper.HashPassword(password, salt)
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    private sealed class TestCurrentUserService : ICurrentUserService
    {
        public TestCurrentUserService(int? userId)
        {
            UserId = userId;
        }

        public int? UserId { get; }

        public string? Email => null;

        public int? GetUserIdOrNull() => UserId;

        public int GetUserIdOrThrow() => UserId ?? throw new InvalidOperationException("No current user.");
    }
}
