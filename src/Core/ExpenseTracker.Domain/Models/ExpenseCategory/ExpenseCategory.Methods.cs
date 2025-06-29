
using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Domain.Models;

public partial class ExpenseCategory : Entity<ExpenseCategoryId>
{
    private ExpenseCategory(ExpenseCategoryId id) : base(id) { }

    public ExpenseCategory(string name, bool isSystemCategory, UserId? userId = default) : base(ExpenseCategoryId.Create())
    {
        Name = name;
        IsSystemCategory = isSystemCategory;
        CategoryOwnerId = userId;
    }

    public void UpdateName(string name)
    {
        Name = name;
    }

    public new void MarkAsDeleted()
    {
        if(Expenses.Any())
        {
            throw new InvalidOperationException("Cannot delete an expense category that has associated expenses.");
        }
        base.MarkAsDeleted();
    }
}
