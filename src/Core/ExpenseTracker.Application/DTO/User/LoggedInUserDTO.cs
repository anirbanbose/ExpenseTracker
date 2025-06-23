using System.Security.Claims;

namespace ExpenseTracker.Application.DTO.User;

public record LoggedInUserDTO(string Email, string FullName)
{
    public static LoggedInUserDTO FromDomain(Domain.Models.User user)
    {
        return new LoggedInUserDTO(user.Email, user.Name.ToString());
    }

    public Claim[] ToClaims()
    {
        return new[]
        {
            new Claim(ClaimTypes.Name, Email)
        };
    }
}
