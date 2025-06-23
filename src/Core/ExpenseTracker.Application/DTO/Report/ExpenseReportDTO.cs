
namespace ExpenseTracker.Application.DTO.Report;

public record ExpenseReportDTO(DateTime ExpenseDate, string Category, string? Description, string ExpenseAmount)
{
    public static ExpenseReportDTO FromDomain(Domain.Models.Expense expense)
    {
        return new ExpenseReportDTO(
            expense.ExpenseDate,
            expense.Category.Name,
            expense.Description,
            expense.ExpenseAmount.FormattedAmount);
    }
}
