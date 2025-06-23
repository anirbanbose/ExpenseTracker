using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Models;
using ExpenseTracker.Domain.Persistence.SearchModels;
using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Domain.Persistence.Repositories;

public interface IExpenseRepository
{
    Task AddExpenseAsync(Expense expense);
    void UpdateExpense(Expense expense);
    Task<Expense?> GetExpenseByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<Expense>> GetExpensesByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<PagedResult<Expense>> SearchExpensesAsync(ExpenseSearchModel search, Guid userId, int PageIndex, int PageSize, ExpenseListOrder Order, bool IsAscendingSort, CancellationToken cancellationToken);
    Task<IEnumerable<Expense>> SearchExpensesAsync(ExpenseSearchModel search, Guid userId, ExpenseListOrder Order, bool IsAscendingSort, CancellationToken cancellationToken);
    Task<IEnumerable<Expense>> GetRecentExpensesAsync(Guid userId, int recordCount, CancellationToken cancellationToken);
    Task<IEnumerable<Expense>> GetLast12MonthsExpensesAsync(Guid userId, CancellationToken cancellationToken);
}
