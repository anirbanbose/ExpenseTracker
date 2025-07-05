
namespace ExpenseTracker.Application.DTO.UserPreference;

public record UserPreferenceDTO(bool EnableMonthlyExpenseReportMail, bool EnableDailyExpenseReportMail)
{
    public static UserPreferenceDTO FromDomain(Domain.Models.UserPreference userPreference)
    {
        return new UserPreferenceDTO(userPreference.SendMonthlyExpenseReportMail, userPreference.SendDailyExpenseReportMail);
    }
}
