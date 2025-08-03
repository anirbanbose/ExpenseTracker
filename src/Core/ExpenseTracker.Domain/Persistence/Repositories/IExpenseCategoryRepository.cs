using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Models;
namespace ExpenseTracker.Domain.Persistence.Repositories;

public interface IExpenseCategoryRepository
{
    Task AddExpenseCategoryAsync(ExpenseCategory expenseCategory);
    void UpdateExpenseCategory(ExpenseCategory expenseCategory);
    Task<ExpenseCategory?> GetExpenseCategoryByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<ExpenseCategory?> GetExpenseCategoryByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<(int TotalCount, IEnumerable<ExpenseCategory> Items)> SearchUserExpenseCategoriesAsync(string? searchText, Guid userId, int pageIndex, int pageSize, ExpenseCategoryListOrder order, bool isAscendingSort, CancellationToken cancellationToken);
    Task<IEnumerable<ExpenseCategory>> GetAllExpenseCategoriesByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<ExpenseCategory>> GetAllSystemExpenseCategoriesAsync(CancellationToken cancellationToken);
}
