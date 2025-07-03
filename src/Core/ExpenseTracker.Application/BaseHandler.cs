using ExpenseTracker.Domain.Models;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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

    
    protected async Task<(User? user, T? failureResult)> GetAuthenticatedUserAsync<T>(IUserRepository userRepository, ILogger logger, CancellationToken cancellationToken, IUserNotAuthenticatedResultFactory<T> factory)
    {
        if (!IsCurrentUserAuthenticated || string.IsNullOrEmpty(CurrentUserName))
        {
            logger.LogWarning("Current user name is null or empty.");
            return (null, factory.UserNotAuthenticatedResult());
        }

        var user = await userRepository.GetUserByEmailAsync(CurrentUserName, cancellationToken);
        if (user is null || user.Deleted)
        {
            logger.LogWarning($"User - {CurrentUserName} is not authenticated.");
            return (null, factory.UserNotAuthenticatedResult());
        }

        return (user, default);
    }

}
