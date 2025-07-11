
using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Domain.Models;

public partial class Expense : Entity<ExpenseId>
{
    private Expense(ExpenseId id) : base(id) { }

    public Expense(Money amount, string? description, ExpenseCategory category, DateTime expenseDate, UserId userId) : base(ExpenseId.Create())
    {
        ExpenseAmount = amount;
        Description = description;
        Category = category;
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
    public void UpdateCategory(ExpenseCategory category)
    {
        Category = category;
    }
    public void UpdateExpenseDate(DateTime expenseDate)
    {
        ExpenseDate = expenseDate;
    }

    public void Update(Money amount, string? description, ExpenseCategory category, DateTime expenseDate)
    {
        UpdateAmount(amount);
        UpdateDescription(description);
        UpdateCategory(category);
        UpdateExpenseDate(expenseDate);
    }
}