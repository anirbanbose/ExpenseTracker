using ExpenseTracker.Application.DTO.Currency;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Currency.Queries;

public class GetAllCurrenciesQueryHandler : IRequestHandler<GetAllCurrenciesQuery, Result<List<CurrencyDTO>>>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ILogger<GetAllCurrenciesQueryHandler> _logger;

    public GetAllCurrenciesQueryHandler(ICurrencyRepository currencyRepository, ILogger<GetAllCurrenciesQueryHandler> logger) 
    {
        _currencyRepository = currencyRepository;
        _logger = logger;
    }

    public async Task<Result<List<CurrencyDTO>>> Handle(GetAllCurrenciesQuery request, CancellationToken cancellationToken)
    { 
        try
        {
            var currencies = await _currencyRepository.GetAllCurrenciesAsync(cancellationToken);
            if (currencies is null || !currencies.Any())
            {
                _logger.LogWarning("No currencies found in the database.");
                return Result<List<CurrencyDTO>>.NotFoundResult();
            }
            var list = currencies.Select(c => CurrencyDTO.FromDomain(c)).ToList();

            return Result<List<CurrencyDTO>>.SuccessResult(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while handling GetAllCurrenciesQuery.");
        }
        return Result<List<CurrencyDTO>>.FailureResult("Currency.GetAllCurrencies");
    }

}
