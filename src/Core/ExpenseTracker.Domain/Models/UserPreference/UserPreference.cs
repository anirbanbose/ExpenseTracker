using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Domain.Models;

public partial class UserPreference : Entity<UserId>
{
    public override UserId Id { get; protected set; }
    public CurrencyId PreferredCurrencyId { get; private set; }
    public Currency PreferredCurrency { get; private set; }

    public bool SendMonthlyExpenseReportMail { get; private set; } = false;
    public bool SendDailyExpenseReportMail { get; private set; } = false;

    public User User { get; private set; }
}
