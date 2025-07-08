using ExpenseTracker.Application.DTO.User;

namespace ExpenseTracker.Application.Contracts.Auth;

public interface ITokenManager
{
    bool GenerateTokensAndSetCookies(LoggedInUserDTO user, bool isPersistent);
}
