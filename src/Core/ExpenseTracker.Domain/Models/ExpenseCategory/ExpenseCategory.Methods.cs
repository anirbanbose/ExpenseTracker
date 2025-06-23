
namespace ExpenseTracker.Domain.Models;

public partial class ExpenseCategory
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
}
