namespace TravelSBE.Services;

using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TravelsBE.Services.Interfaces;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public int? UserId
    {
        get
        {
            var user = User;
            if (user == null) return null;

            var claim = user.FindFirst(ClaimTypes.NameIdentifier)
                        ?? user.FindFirst("sub")
                        ?? user.FindFirst(ClaimTypes.Name);

            if (claim == null) return null;

            return int.TryParse(claim.Value, out var id) ? id : null;
        }
    }

    public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;

    public int? GetUserIdOrNull() => UserId;

    public int GetUserIdOrThrow()
    {
        var id = UserId;
        if (id == null) throw new InvalidOperationException("No authenticated user present.");
        return id.Value;
    }
}