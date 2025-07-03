using ExpenseTracker.Application.Validation;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Application.UseCases.UserPreference.Commands;

public class SaveUserPreferenceCommand : IRequest<Result>
{
    [Required(ErrorMessage = "Please select expense category")]
    [NotEmptyGuid(ErrorMessage = "Please select expense category")]
    public Guid PreferredCurrencyId { get; set; }
    public bool EnableMonthlyExpenseReportMail { get; set; } = false;
    public bool EnableDailyExpenseReportMail { get; set; } = false;
}
