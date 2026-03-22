namespace TravelsBE.Services.Interfaces
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string? Email { get; }

        int? GetUserIdOrNull();
        int GetUserIdOrThrow();
    }
}
