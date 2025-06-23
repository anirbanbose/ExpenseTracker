using ExpenseTracker.Application.DTO.User;

namespace ExpenseTracker.Application.Contracts.Auth;

public interface IAuthManager
{
    bool GenerateTokensAndSetCookies(LoggedInUserDTO user, bool isPersistent);
}
