using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Domain.Models;

public partial class Expense : Entity<ExpenseId>
{
    public Money ExpenseAmount { get; private set; }
    public string? Description { get; private set; }
    public DateTime ExpenseDate { get; private set; }
    public ExpenseCategoryId CategoryId { get; private set; }
    public UserId ExpenseOwnerId { get; private set; }

    public User ExpenseOwner { get; private set; }
    public ExpenseCategory Category { get; private set; }
}
