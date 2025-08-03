using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Models;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Persistence.Repositories;

public class ExpenseCategoryRepository(ApplicationDbContext dbContext) : BaseRepository<ExpenseCategory>(dbContext), IExpenseCategoryRepository
{
    public async Task AddExpenseCategoryAsync(ExpenseCategory expenseCategory)
    {
        if (expenseCategory is null)
        {
            throw new ArgumentNullException(nameof(expenseCategory));
        }
        await base.AddAsync(expenseCategory);
    }

    public async Task<IEnumerable<ExpenseCategory>> GetAllExpenseCategoriesByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await Table.Where(d => d.IsSystemCategory == true || (d.CategoryOwnerId != null && d.CategoryOwnerId == userId) && !d.Deleted).OrderBy(d => d.Name).ToListAsync(cancellationToken);
    }

    public async Task<ExpenseCategory?> GetExpenseCategoryByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        return await Table.Include(d => d.Expenses).FirstOrDefaultAsync(d => d.Id == id && d.CategoryOwnerId != null && d.CategoryOwnerId == userId && !d.Deleted, cancellationToken);
    }
    public async Task<ExpenseCategory?> GetExpenseCategoryByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await Table.FirstOrDefaultAsync(d => d.Id == id && !d.Deleted, cancellationToken);
    }

    public async Task<IEnumerable<ExpenseCategory>> GetAllSystemExpenseCategoriesAsync(CancellationToken cancellationToken)
    {
        return await Table.Where(d => d.IsSystemCategory == true && !d.Deleted).OrderBy(d => d.Name).ToListAsync(cancellationToken);
    }

    public async Task<(int TotalCount, IEnumerable<ExpenseCategory> Items)> SearchUserExpenseCategoriesAsync(string? searchText, Guid userId, int pageIndex, int pageSize, ExpenseCategoryListOrder order, bool isAscendingSort, CancellationToken cancellationToken)
    {
        string searchString = !string.IsNullOrWhiteSpace(searchText) ? searchText.Trim().ToLower() : string.Empty;

        IQueryable<ExpenseCategory> query = TableNoTracking;

        query = query.Where(d => d.IsSystemCategory == false && d.CategoryOwnerId != null && d.CategoryOwnerId == userId && !d.Deleted
                                &&
                                (string.IsNullOrEmpty(searchString)
                                || d.Name.ToLower().Contains(searchString)
                                ));

        var totalCount = await query.CountAsync();

        query = query.Include(d => d.CategoryOwner)
                     .Include(d => d.Expenses)
                     .AsSplitQuery().AsQueryable();

        var orderByExprssion = order.ToOrderExpression();
        query = isAscendingSort ? query.OrderBy(orderByExprssion) : query.OrderByDescending(orderByExprssion);

        var items = await query.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return (totalCount, items);
    }

    public void UpdateExpenseCategory(ExpenseCategory expenseCategory)
    {
        if (expenseCategory is null)
        {
            throw new ArgumentNullException(nameof(expenseCategory));
        }
        MarkAsUpdated(expenseCategory);
    }
}
