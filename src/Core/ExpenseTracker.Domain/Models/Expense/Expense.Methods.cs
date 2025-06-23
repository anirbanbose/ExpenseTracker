
using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Domain.Models;

public partial class Expense : Entity<ExpenseId>
{
    private Expense(ExpenseId id) : base(id) { }

    public Expense(Money amount, string? description, ExpenseCategoryId categoryId, DateTime expenseDate, UserId userId) : base(ExpenseId.Create())
    {
        ExpenseAmount = amount;
        Description = description;
        CategoryId = categoryId;
        ExpenseDate = expenseDate;
        ExpenseOwnerId = userId;
    }

    public void UpdateAmount(Money amount)
    {
        ExpenseAmount = amount;
    }
    public void UpdateDescription(string? description)
    {
        Description = description;
    }
    public void UpdateCategory(ExpenseCategoryId categoryId)
    {
        CategoryId = categoryId;
    }
    public void UpdateExpenseDate(DateTime expenseDate)
    {
        ExpenseDate = expenseDate;
    }

    public void Update(Money amount, string? description, ExpenseCategoryId categoryId, DateTime expenseDate)
    {
        UpdateAmount(amount);
        UpdateDescription(description);
        UpdateCategory(categoryId);
        UpdateExpenseDate(expenseDate);
    }
}