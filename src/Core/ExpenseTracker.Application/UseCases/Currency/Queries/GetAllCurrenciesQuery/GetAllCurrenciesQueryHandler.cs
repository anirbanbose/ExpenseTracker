using ExpenseTracker.Application.DTO.Currency;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Currency.Queries;

public class GetAllCurrenciesQueryHandler : BaseHandler, IRequestHandler<GetAllCurrenciesQuery, Result<List<CurrencyDTO>>>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ILogger<GetAllCurrenciesQueryHandler> _logger;

    public GetAllCurrenciesQueryHandler(IHttpContextAccessor httpContextAccessor, ICurrencyRepository currencyRepository, ILogger<GetAllCurrenciesQueryHandler> logger) : base(httpContextAccessor)
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
                return Result<List<CurrencyDTO>>.NotFoundResult("No currencies found.");
            }
            var list = currencies.Select(c => CurrencyDTO.FromDomain(c)).ToList();

            return Result<List<CurrencyDTO>>.SuccessResult(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while handling GetAllCurrenciesQuery.");
        }
        return Result<List<CurrencyDTO>>.FailureResult("Currency.UnknownException", "An error occurred while processing your request.");
    }

}
