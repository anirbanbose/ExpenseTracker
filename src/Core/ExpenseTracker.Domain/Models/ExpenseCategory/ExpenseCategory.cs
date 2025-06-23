using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Domain.Models;

public partial class ExpenseCategory : Entity<ExpenseCategoryId>
{
    private List<Expense> _expenses = [];
    public string Name { get; private set; }
    public bool IsSystemCategory { get; set; }
    public UserId? CategoryOwnerId { get; private set; } = default!;
    public User? CategoryOwner { get; private set; } = default!;
    public IReadOnlyCollection<Expense> Expenses => _expenses.ToList();
}
