using ExpenseTracker.Application.DTO.ExpenseCategory;

namespace ExpenseTracker.Application.DTO.Expense;

public record ExpenseDTO(Guid Id, string? Description, decimal ExpenseAmount, DateTime ExpenseDate, ExpenseCategoryDTO Category, Guid CurrencyId)
{
    public static ExpenseDTO FromDomain(Domain.Models.Expense expense)
    {
        return new ExpenseDTO(
            expense.Id, 
            expense.Description, 
            expense.ExpenseAmount.Amount, 
            expense.ExpenseDate, 
            new ExpenseCategoryDTO(expense.Category.Id, expense.Category.Name, expense.Category.IsSystemCategory, expense.Category.Expenses.Count ), 
            expense.ExpenseAmount.CurrencyId);
    }
}