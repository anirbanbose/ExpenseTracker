using ExpenseTracker.Application.Contracts.Auth;
using System.Security.Principal;

namespace ExpenseTracker.Presentation.Api;

public class CurrenctUserManager : ICurrentUserManager
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrenctUserManager(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private IIdentity? CurrentUserIdentity => _httpContextAccessor.HttpContext?.User.Identity;
    public string? CurrentUserName => CurrentUserIdentity?.Name;

    public bool IsAuthenticated => CurrentUserIdentity?.IsAuthenticated ?? false;
}
