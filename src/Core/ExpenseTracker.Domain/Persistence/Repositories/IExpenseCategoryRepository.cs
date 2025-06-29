using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Models;
using ExpenseTracker.Domain.Persistence.SearchModels;
using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Domain.Persistence.Repositories;

public interface IExpenseCategoryRepository
{
    Task AddExpenseCategoryAsync(ExpenseCategory expenseCategory);
    void UpdateExpenseCategory(ExpenseCategory expenseCategory);
    Task<ExpenseCategory?> GetExpenseCategoryByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<ExpenseCategory?> GetExpenseCategoryByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<ExpenseCategory>> SearchUserExpenseCategoriesAsync(ExpenseCategorySearchModel search, Guid userId, int PageIndex, int PageSize, ExpenseCategoryListOrder Order, bool IsAscendingSort, CancellationToken cancellationToken);
    Task<IEnumerable<ExpenseCategory>> GetAllExpenseCategoriesByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<ExpenseCategory>> GetAllSystemExpenseCategoriesAsync(CancellationToken cancellationToken);
}
