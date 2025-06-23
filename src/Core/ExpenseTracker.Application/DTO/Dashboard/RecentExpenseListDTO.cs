namespace ExpenseTracker.Application.DTO.Dashboard;

public record RecentExpenseListDTO(DateTime ExpenseDate, string ExpenseCategory, string ExpenseAmount)
{
    public static RecentExpenseListDTO FromDomain(Domain.Models.Expense expense)
    {
        return new RecentExpenseListDTO(expense.ExpenseDate, expense.Category.Name, expense.ExpenseAmount.FormattedAmount);
    }
}
