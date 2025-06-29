
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Models;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Persistence.Repositories;

public class CurrencyRepository(ApplicationDbContext dbContext) : BaseRepository<Currency>(dbContext), ICurrencyRepository
{
    public async Task AddCurrencyAsync(Currency currency)
    {
        if (currency is null)
        {
            throw new ArgumentNullException(nameof(currency));
        }
        await base.AddAsync(currency);
    }

    public async Task<IEnumerable<Currency>> GetAllCurrenciesAsync(CancellationToken cancellationToken)
    {
        return await Table.ToListAsync(cancellationToken);
    }

    public async Task<Currency?> GetCurrencyByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await Table.FirstOrDefaultAsync(d => d.Code.ToLower().Equals(code.ToLower()), cancellationToken);
    }

    public async Task<Currency?> GetCurrencyByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await Table.FirstOrDefaultAsync(d => d.Id == id && !d.Deleted, cancellationToken);
    }

    public async Task<PagedResult<Currency>> SearchCurrenciesAsync(string? search, int pageIndex, int pageSize, CurrencyListOrder order, bool isAscendingSort, CancellationToken cancellationToken)
    {
        string searchString = !string.IsNullOrWhiteSpace(search) ? search.Trim().ToLower() : string.Empty;

        IQueryable<Currency> query = TableNoTracking;

        query = query.Where(d => (string.IsNullOrEmpty(searchString)
                                || d.Name.ToLower().Contains(searchString)
                                || d.Code.ToLower().Contains(searchString)
                                || (!string.IsNullOrEmpty(d.Symbol) && d.Symbol.ToLower().Contains(searchString))
                                ));

        var totalCount = await query.CountAsync();
        

        var orderByExprssion = order.ToOrderExpression();
        query = isAscendingSort ? query.OrderBy(orderByExprssion) : query.OrderByDescending(orderByExprssion);

        var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return PagedResult<Currency>.SuccessResult(items, totalCount, pageIndex, pageSize);
    }

    public void UpdateCurrency(Currency currency)
    {
        if (currency is null)
        {
            throw new ArgumentNullException(nameof(currency));
        }
        MarkAsUpdated(currency);
    }
}
