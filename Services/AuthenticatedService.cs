namespace TravelSBE.Services;

using TravelsBE.Services.Interfaces;

public abstract class AuthenticatedService
{
    protected readonly ICurrentUserService CurrentUserService;

    protected AuthenticatedService(ICurrentUserService currentUserService)
    {
        CurrentUserService = currentUserService;
    }

    protected int GetCurrentUserIdOrThrow()
    {
        return CurrentUserService.GetUserIdOrThrow();
    }

    protected int? TryGetCurrentUserId()
    {
        return CurrentUserService.GetUserIdOrNull();
    }
}
