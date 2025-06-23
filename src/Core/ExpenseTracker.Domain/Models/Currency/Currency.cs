using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Domain.Models;

public partial class Currency : Entity<CurrencyId>
{
    //private List<UserPreference> _users = [];
    public string Code { get; private set; }  // "USD", "EUR", etc.
    public string? Symbol { get; private set; } // "$", "€"
    public string Name { get; private set; } // "US Dollar", "Euro"

    //public IReadOnlyCollection<UserPreference> Users => _users.ToList();
}
