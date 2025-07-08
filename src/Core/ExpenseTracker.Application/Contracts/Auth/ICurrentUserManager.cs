

namespace ExpenseTracker.Application.Contracts.Auth;

public interface ICurrentUserManager
{
    string? CurrentUserName { get; }
    bool IsAuthenticated { get; }
}
