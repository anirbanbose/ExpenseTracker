

namespace ExpenseTracker.Application.DTO.Profile;

public record UserProfileDTO(string Email, string FirstName, string LastName, string? MiddleName)
{
    public static UserProfileDTO FromDomain(Domain.Models.User user)
    {
        return new UserProfileDTO(user.Email, user.Name.FirstName, user.Name.LastName, user.Name.MiddleName);
    }
}