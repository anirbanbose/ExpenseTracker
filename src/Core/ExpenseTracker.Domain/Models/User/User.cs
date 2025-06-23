using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Domain.Models;

public partial class User : Entity<UserId>
{
    private List<ExpenseCategory> _expenseCategories = [];
    private List<Expense> _expenses = [];
    public PersonName Name { get; private set; }
    public string Email { get; init; }
    public string PasswordHash { get; private set; }
    public string PasswordSalt { get; private set; }
    public UserPreference? Preference { get; private set; }
    public IReadOnlyCollection<Expense> Expenses => _expenses.ToList();
    public IReadOnlyCollection<ExpenseCategory> ExpenseCategories => _expenseCategories.ToList();
}
