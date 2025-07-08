using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Domain.Models;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application;

public abstract class BaseHandler
{
    private readonly ICurrentUserManager _currentUserManager;
    protected BaseHandler(ICurrentUserManager currentUserManager)
    {
        _currentUserManager = currentUserManager;
    }

    protected string? CurrentUserName => _currentUserManager?.CurrentUserName;
    protected bool IsCurrentUserAuthenticated => _currentUserManager.IsAuthenticated;

    
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
