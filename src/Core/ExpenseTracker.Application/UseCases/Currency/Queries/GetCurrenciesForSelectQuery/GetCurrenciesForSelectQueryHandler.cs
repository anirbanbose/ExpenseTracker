using ExpenseTracker.Application.DTO.Currency;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Currency.Queries;

public class GetCurrenciesForSelectQueryHandler : IRequestHandler<GetCurrenciesForSelectQuery, Result<List<CurrencyForSelectDTO>>>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ILogger<GetCurrenciesForSelectQueryHandler> _logger;


    public GetCurrenciesForSelectQueryHandler(ICurrencyRepository currencyRepository, ILogger<GetCurrenciesForSelectQueryHandler> logger) 
    {
        _currencyRepository = currencyRepository;
        _logger = logger;
    }

    public async Task<Result<List<CurrencyForSelectDTO>>> Handle(GetCurrenciesForSelectQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var currencies = await _currencyRepository.GetAllCurrenciesAsync(cancellationToken);
            if (currencies is null || !currencies.Any())
            {
                _logger.LogWarning("No currencies found in the database.");
                return Result<List<CurrencyForSelectDTO>>.NotFoundResult();
            }
            var list = currencies.Select(c => CurrencyForSelectDTO.FromDomain(c)).ToList();

            return Result<List<CurrencyForSelectDTO>>.SuccessResult(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching all currencies.");
        }
        return Result<List<CurrencyForSelectDTO>>.FailureResult("Currency.GetCurrenciesForSelect");

    }
}
