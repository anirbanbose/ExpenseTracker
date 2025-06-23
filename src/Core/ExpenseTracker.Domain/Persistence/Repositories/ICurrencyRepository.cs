
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Models;
using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Domain.Persistence.Repositories;

public interface ICurrencyRepository
{
    Task AddCurrencyAsync(Currency currency);
    void UpdateCurrency(Currency currency);
    Task<Currency?> GetCurrencyByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Currency?> GetCurrencyByCodeAsync(string code, CancellationToken cancellationToken);
    Task<PagedResult<Currency>> SearchCurrenciesAsync(string? search, int pageIndex, int pageSize, CurrencyListOrder order, bool isAscendingSort, CancellationToken cancellationToken);
    Task<IEnumerable<Currency>> GetAllCurrenciesAsync(CancellationToken cancellationToken);
}
