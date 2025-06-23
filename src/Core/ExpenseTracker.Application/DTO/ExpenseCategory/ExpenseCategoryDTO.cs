namespace ExpenseTracker.Application.DTO.ExpenseCategory;

public record ExpenseCategoryDTO(Guid Id, string Name, bool IsSystemCategory, int ExpenseCount)
{
    public static ExpenseCategoryDTO FromDomain(Domain.Models.ExpenseCategory expenseCategory)
    {
        return new ExpenseCategoryDTO(expenseCategory.Id, expenseCategory.Name, expenseCategory.IsSystemCategory, expenseCategory.Expenses.Count);
    }
}
