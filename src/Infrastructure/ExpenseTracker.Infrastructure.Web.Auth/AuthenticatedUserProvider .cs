using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Domain.Models;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Principal;

namespace ExpenseTracker.Infrastructure.Web.Auth;

public class AuthenticatedUserProvider : IAuthenticatedUserProvider
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthenticatedUserProvider> _logger;

    private IIdentity? CurrentUserIdentity => _httpContextAccessor.HttpContext?.User.Identity;
    public string? CurrentUserName => CurrentUserIdentity?.Name;

    public bool IsCurrentUserAuthenticated => CurrentUserIdentity?.IsAuthenticated ?? false;

    public AuthenticatedUserProvider(
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository,
        ILogger<AuthenticatedUserProvider> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<(User? user, T? failureResult)> GetAuthenticatedUserAsync<T>(IUserNotAuthenticatedResultFactory<T> factory, CancellationToken cancellationToken) where T : class
    {
        if (!IsCurrentUserAuthenticated || string.IsNullOrEmpty(CurrentUserName))
        {
            _logger.LogWarning("User not authenticated or Current user name is null or empty.");
            return (null, factory.UserNotAuthenticatedResult());
        }

        var user = await _userRepository.GetUserByEmailAsync(CurrentUserName, cancellationToken);
        if (user is null || user.Deleted)
        {
            _logger.LogWarning($"User name - {CurrentUserName} doesn't exist or deleted.");
            return (null, factory.UserNotAuthenticatedResult());
        }

        return (user, default);
    }
}
