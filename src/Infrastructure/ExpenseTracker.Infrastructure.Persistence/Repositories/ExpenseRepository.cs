using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Models;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Persistence.Repositories;

public class ExpenseRepository(ApplicationDbContext dbContext) : BaseRepository<Expense>(dbContext), IExpenseRepository
{
    public async Task AddExpenseAsync(Expense expense)
    {
        if (expense is null)
        {
            throw new ArgumentNullException(nameof(expense));
        }
        await base.AddAsync(expense);
    }

    public async Task<Expense?> GetExpenseByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        return await Table
            .Include(d => d.Category)
            .Include(d => d.ExpenseOwner).ThenInclude(o => o.Preference).ThenInclude(p => p.PreferredCurrency)
            .AsSplitQuery().FirstOrDefaultAsync(d => d.Id == id && !d.Deleted && d.ExpenseOwnerId == userId, cancellationToken);
    }

    public async Task<IEnumerable<Expense>> GetExpensesByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await TableNoTracking
            .Include(d => d.Category)
            .Where(d => d.ExpenseOwnerId == userId && !d.Deleted)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    public async Task<(int TotalCount, IEnumerable<Expense> Items)> SearchExpensesAsync(string? searchText, Guid? expenseCategoryId, DateTime? startDate, DateTime? endDate, Guid userId, int pageIndex, int pageSize, ExpenseListOrder order, bool isAscendingSort, CancellationToken cancellationToken)
    {
        string searchString = !string.IsNullOrWhiteSpace(searchText) ? searchText.Trim().ToLower() : string.Empty;

        IQueryable<Expense> query = TableNoTracking;
        query = query.Include(d => d.ExpenseOwner)
                        .Include(d => d.Category);
        query = query.Where(d => d.ExpenseOwnerId == userId && !d.Deleted
                                && (string.IsNullOrEmpty(searchString)
                                    || (d.Description != null && d.Description.ToLower().Contains(searchString))
                                    || d.Category.Name.ToLower().Contains(searchString)
                                    || d.ExpenseAmount.Amount.ToString().Contains(searchString)
                                    )
                                && (expenseCategoryId == null || d.CategoryId == expenseCategoryId)
                                && (startDate == null || d.ExpenseDate.Date >= startDate.Value.Date)
                                && (endDate == null || d.ExpenseDate.Date <= endDate.Value.Date)
                                );

        var totalCount = await query.CountAsync();

        query = query.AsSplitQuery().AsQueryable();

        var orderByExprssion = order.ToOrderExpression();
        query = isAscendingSort ? query.OrderBy(orderByExprssion).ThenByDescending(d => d.CreatedDate) : query.OrderByDescending(orderByExprssion).ThenByDescending(d => d.CreatedDate);
        
        var items = await query.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return (totalCount, items);
    }

    public async Task<IEnumerable<Expense>> SearchExpensesAsync(string? searchText, Guid? expenseCategoryId, DateTime? startDate, DateTime? endDate, Guid userId, ExpenseListOrder order, bool isAscendingSort, CancellationToken cancellationToken)
    {
        string searchString = !string.IsNullOrWhiteSpace(searchText) ? searchText.Trim().ToLower() : string.Empty;

        IQueryable<Expense> query = TableNoTracking;
        query = query.Include(d => d.ExpenseOwner)
                        .Include(d => d.Category);
        query = query.Where(d => d.ExpenseOwnerId == userId && !d.Deleted
                                && (string.IsNullOrEmpty(searchString)
                                    || (d.Description != null && d.Description.ToLower().Contains(searchString))
                                    || d.Category.Name.ToLower().Contains(searchString)
                                    || d.ExpenseAmount.Amount.ToString().Contains(searchString))
                                && (expenseCategoryId == null || d.CategoryId == expenseCategoryId)
                                && (startDate == null || d.ExpenseDate.Date >= startDate.Value.Date)
                                && (endDate == null || d.ExpenseDate.Date <= endDate.Value.Date)
                                );

        var totalCount = await query.CountAsync();

        query = query.AsSplitQuery().AsQueryable();

        var orderByExprssion = order.ToOrderExpression();
        query = isAscendingSort ? query.OrderBy(orderByExprssion).ThenByDescending(d => d.CreatedDate) : query.OrderByDescending(orderByExprssion).ThenByDescending(d => d.CreatedDate);
        
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<(int TotalCount, int PageIndex, IEnumerable<Expense> Items)> SearchExpensesAsync(Guid expenseId, Guid userId, int pageSize, CancellationToken cancellationToken)
    {
        IQueryable<Expense> query = TableNoTracking;
        query = query.Include(d => d.ExpenseOwner)
                        .Include(d => d.Category);
        query = query.Where(d => d.ExpenseOwnerId == userId && !d.Deleted).OrderByDescending(d => d.ExpenseDate).ThenByDescending(d => d.CreatedDate);

        var item = await query.FirstOrDefaultAsync(d => d.Id == expenseId);

        int itemIndex = query.ToList().FindIndex(e => e.Id == expenseId);
        int pageIndex = (int)Math.Ceiling((double)(itemIndex + 1) / pageSize);


        var totalCount = await query.CountAsync();

        query = query.AsSplitQuery().AsQueryable();

        var items = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize).ToListAsync(cancellationToken);

        return (totalCount, pageIndex, items);
    }

    public async Task<IEnumerable<Expense>> GetRecentExpensesAsync(Guid userId, int recordCount, CancellationToken cancellationToken)
    {
        IQueryable<Expense> query = TableNoTracking;
        query = query.Include(d => d.ExpenseOwner)
                        .Include(d => d.Category);
        query = query.Where(d => d.ExpenseOwnerId == userId && !d.Deleted);

        var totalCount = await query.CountAsync();

        query = query.AsSplitQuery().AsQueryable().OrderByDescending(d => d.ExpenseDate).ThenByDescending(d => d.CreatedDate);

        return await query.Take(recordCount).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Expense>> GetLast12MonthsExpensesAsync(Guid userId, CancellationToken cancellationToken)
    {
        IQueryable<Expense> query = TableNoTracking;
        query = query.Include(d => d.ExpenseOwner)
                        .Include(d => d.Category);
        query = query.Where(d => d.ExpenseOwnerId == userId && !d.Deleted);
        var endDate = query.Any() ? await query.MaxAsync(e => e.ExpenseDate) : DateTime.UtcNow.Date;

        var startDate = new DateTime(endDate.Year, endDate.Month, 1).AddMonths(-11);

        return await query
            .Where(e => e.ExpenseDate >= startDate && e.ExpenseDate <= endDate)
            .ToListAsync(cancellationToken);
    }
    public void UpdateExpense(Expense expense)
    {
        if (expense is null)
        {
            throw new ArgumentNullException(nameof(expense));
        }
        MarkAsUpdated(expense);
    }

}
