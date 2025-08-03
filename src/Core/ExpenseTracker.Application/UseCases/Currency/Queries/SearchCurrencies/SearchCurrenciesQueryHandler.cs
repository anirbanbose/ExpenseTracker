using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.Currency;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Currency.Queries;

public class SearchCurrenciesQueryHandler : IRequestHandler<SearchCurrenciesQuery, PagedResult<CurrencyDTO>>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly ILogger<SearchCurrenciesQueryHandler> _logger;

    public SearchCurrenciesQueryHandler(IAuthenticatedUserProvider authProvider, ICurrencyRepository currencyRepository, ILogger<SearchCurrenciesQueryHandler> logger)
    {
        _currencyRepository = currencyRepository;
        _authProvider = authProvider;
        _logger = logger;
    }

    public async Task<PagedResult<CurrencyDTO>> Handle(SearchCurrenciesQuery request, CancellationToken cancellationToken)
    {        
        try
        {
            var (currentUser, pagedFailureResult) = await _authProvider.GetAuthenticatedUserAsync<PagedResult<CurrencyDTO>>(new PagedResultUserNotAuthenticatedFactory<CurrencyDTO>(), cancellationToken);
            if (pagedFailureResult != null)
                return pagedFailureResult;

            var currenciesResult = await _currencyRepository.SearchCurrenciesAsync(request.search, request.pageIndex, request.pageSize, request.order, request.isAscendingSort, cancellationToken);
            if (currenciesResult.Items is null)
            {
                return PagedResult<CurrencyDTO>.FailureResult();
            }
            
            List<CurrencyDTO> dtoList = new List<CurrencyDTO>();
            currenciesResult.Items.ToList().ForEach(currency =>
            {
                dtoList.Add(CurrencyDTO.FromDomain(currency));
            });
            return PagedResult<CurrencyDTO>.SuccessResult(dtoList, currenciesResult.TotalCount, request.pageIndex, request.pageSize);
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while handling SearchCurrenciesQuery for the user: {_authProvider.CurrentUserName}.");
        }
       
        return PagedResult<CurrencyDTO>.FailureResult();
    }

}