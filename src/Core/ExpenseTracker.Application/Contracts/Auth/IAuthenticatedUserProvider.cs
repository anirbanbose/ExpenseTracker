

using ExpenseTracker.Domain.Models;
using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Application.Contracts.Auth;

public interface IAuthenticatedUserProvider
{
    string? CurrentUserName { get; }
    bool IsCurrentUserAuthenticated { get; }
    Task<(User? user, T? failureResult)> GetAuthenticatedUserAsync<T>(IUserNotAuthenticatedResultFactory<T> factory, CancellationToken cancellationToken) where T : class;
}
