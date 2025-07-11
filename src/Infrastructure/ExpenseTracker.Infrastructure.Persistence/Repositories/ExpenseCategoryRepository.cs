﻿using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Models;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.Persistence.SearchModels;
using ExpenseTracker.Domain.SharedKernel;
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

    public async Task<PagedResult<ExpenseCategory>> SearchUserExpenseCategoriesAsync(ExpenseCategorySearchModel search, Guid userId, int PageIndex, int PageSize, ExpenseCategoryListOrder Order, bool IsAscendingSort, CancellationToken cancellationToken)
    {
        string searchString = !string.IsNullOrWhiteSpace(search.SearchText) ? search.SearchText.Trim().ToLower() : string.Empty;

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

        var orderByExprssion = Order.ToOrderExpression();
        query = IsAscendingSort ? query.OrderBy(orderByExprssion) : query.OrderByDescending(orderByExprssion);

        var items = await query.Skip(PageIndex * PageSize).Take(PageSize).ToListAsync(cancellationToken);

        return PagedResult<ExpenseCategory>.SuccessResult(items, totalCount, PageIndex, PageSize);
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
