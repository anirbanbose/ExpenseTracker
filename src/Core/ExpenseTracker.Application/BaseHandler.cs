using Microsoft.AspNetCore.Http;
using System.Security.Principal;

namespace ExpenseTracker.Application;

public abstract class BaseHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    protected BaseHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private IIdentity? CurrentUserIdentity => _httpContextAccessor.HttpContext?.User.Identity;
    protected string? CurrentUserName => CurrentUserIdentity?.Name;
    protected bool IsCurrentUserAuthenticated => CurrentUserIdentity?.IsAuthenticated ?? false;

}
