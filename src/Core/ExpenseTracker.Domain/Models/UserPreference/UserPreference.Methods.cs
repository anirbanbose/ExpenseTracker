
using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Domain.Models;

public partial class UserPreference : Entity<UserId>
{
    private UserPreference(UserId id) : base(id) { }

    public static UserPreference Create(UserId userId, Currency preferredCurrency, bool sendMonthlyExpenseReportMail = false, bool sendDailyExpenseReportMail = false)
    {
        UserPreference userPreference = new UserPreference(userId)
        {
            PreferredCurrency = preferredCurrency,
            SendDailyExpenseReportMail = sendDailyExpenseReportMail,
            SendMonthlyExpenseReportMail = sendMonthlyExpenseReportMail,
        };
        return userPreference;
    }

    public void Update(CurrencyId preferredCurrencyId, bool sendMonthlyExpenseReportMail, bool sendDailyExpenseReportMail)
    {
        PreferredCurrencyId = preferredCurrencyId;
        SendMonthlyExpenseReportMail = sendMonthlyExpenseReportMail;
        SendDailyExpenseReportMail = sendDailyExpenseReportMail;
    }
}
