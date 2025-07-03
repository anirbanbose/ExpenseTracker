namespace ExpenseTracker.Application.DTO.Report;

public record ExpenseReportDataDTO(DateTime ExpenseDate, string Category, string? Description, string ExpenseAmount)
{
    public static ExpenseReportDataDTO FromDomain(Domain.Models.Expense expense)
    {
        return new ExpenseReportDataDTO(
            expense.ExpenseDate,
            expense.Category.Name,
            expense.Description,
            expense.ExpenseAmount.FormattedAmount);
    }
}
