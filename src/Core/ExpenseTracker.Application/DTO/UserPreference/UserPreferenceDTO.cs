
namespace ExpenseTracker.Application.DTO.UserPreference;

public record UserPreferenceDTO(Guid PreferredCurrencyId, bool EnableMonthlyExpenseReportMail, bool EnableDailyExpenseReportMail)
{
    public static UserPreferenceDTO FromDomain(Domain.Models.UserPreference userPreference)
    {
        return new UserPreferenceDTO(userPreference.PreferredCurrencyId, userPreference.SendMonthlyExpenseReportMail, userPreference.SendDailyExpenseReportMail);
    }
}
