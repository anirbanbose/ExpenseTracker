namespace ExpenseTracker.Application.DTO.Expense;

public record ExpenseListDTO(Guid Id, string? Description, string ExpenseAmount, DateTime ExpenseDate, string ExpenseCategory)
{
    public static ExpenseListDTO FromDomain(Domain.Models.Expense expense)
    {
        return new ExpenseListDTO(expense.Id, expense.Description, expense.ExpenseAmount.FormattedAmount, expense.ExpenseDate, expense.Category.Name);
    }
}

