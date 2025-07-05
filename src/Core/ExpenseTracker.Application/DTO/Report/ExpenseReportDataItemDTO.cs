namespace ExpenseTracker.Application.DTO.Report;

public record ExpenseReportDataItemDTO(DateTime ExpenseDate, string Category, string? Description, string ExpenseAmount)
{
    public static ExpenseReportDataItemDTO FromDomain(Domain.Models.Expense expense)
    {
        return new ExpenseReportDataItemDTO(
            expense.ExpenseDate,
            expense.Category.Name,
            expense.Description,
            expense.ExpenseAmount.ToString());
    }
}
