using ExpenseTracker.Domain.SharedKernel;
using MediatR;

namespace ExpenseTracker.Application.UseCases.UserPreference.Commands;

public class SaveUserPreferenceCommand : IRequest<Result>
{
    public bool EnableMonthlyExpenseReportMail { get; set; } = false;
    public bool EnableDailyExpenseReportMail { get; set; } = false;
}
