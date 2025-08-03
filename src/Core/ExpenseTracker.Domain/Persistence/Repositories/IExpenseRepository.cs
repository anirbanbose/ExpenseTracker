using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Models;

namespace ExpenseTracker.Domain.Persistence.Repositories;

public interface IExpenseRepository
{
    Task AddExpenseAsync(Expense expense);
    void UpdateExpense(Expense expense);
    Task<Expense?> GetExpenseByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<Expense>> GetExpensesByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<(int TotalCount, IEnumerable<Expense> Items)> SearchExpensesAsync(string? searchText, Guid? expenseCategoryId, DateTime? startDate, DateTime? endDate, Guid userId, int pageIndex, int pageSize, ExpenseListOrder order, bool isAscendingSort, CancellationToken cancellationToken);
    Task<IEnumerable<Expense>> SearchExpensesAsync(string? searchText, Guid? expenseCategoryId, DateTime? startDate, DateTime? endDate, Guid userId, ExpenseListOrder order, bool isAscendingSort, CancellationToken cancellationToken);
    Task<(int TotalCount, int PageIndex, IEnumerable<Expense> Items)> SearchExpensesAsync(Guid id, Guid userId, int pageSize, CancellationToken cancellationToken);
    Task<IEnumerable<Expense>> GetRecentExpensesAsync(Guid userId, int recordCount, CancellationToken cancellationToken);
    Task<IEnumerable<Expense>> GetLast12MonthsExpensesAsync(Guid userId, CancellationToken cancellationToken);    
}
