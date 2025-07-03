using ExpenseTracker.Application.DTO.Currency;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Currency.Queries;

public class SearchCurrenciesQueryHandler : BaseHandler, IRequestHandler<SearchCurrenciesQuery, PagedResult<CurrencyDTO>>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SearchCurrenciesQueryHandler> _logger;

    public SearchCurrenciesQueryHandler(IHttpContextAccessor httpContextAccessor, ICurrencyRepository currencyRepository, IUserRepository userRepository, ILogger<SearchCurrenciesQueryHandler> logger) : base(httpContextAccessor)
    {
        _currencyRepository = currencyRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<PagedResult<CurrencyDTO>> Handle(SearchCurrenciesQuery request, CancellationToken cancellationToken)
    {        
        try
        {
            var (currentUser, pagedFailureResult) = await GetAuthenticatedUserAsync(_userRepository, _logger, cancellationToken, new PagedResultUserNotAuthenticatedFactory<CurrencyDTO>());
            if (pagedFailureResult != null)
                return pagedFailureResult;

            var currenciesResult = await _currencyRepository.SearchCurrenciesAsync(request.search, request.pageIndex, request.pageSize, request.order, request.isAscendingSort, cancellationToken);
            if (!currenciesResult.IsSuccess && currenciesResult.Items.Any())
            {
                return PagedResult<CurrencyDTO>.NotFoundResult("No currencies found.");
            }
            if (currenciesResult.IsSuccess && currenciesResult.Items.Any())
            {
                List<CurrencyDTO> dtoList = new List<CurrencyDTO>();
                currenciesResult.Items.ToList().ForEach(currency =>
                {
                    dtoList.Add(CurrencyDTO.FromDomain(currency));
                });
                return PagedResult<CurrencyDTO>.SuccessResult(dtoList, currenciesResult.TotalCount, currenciesResult.PageIndex, currenciesResult.PageSize);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while handling SearchCurrenciesQuery for the user: {CurrentUserName}.");
        }
       
        return PagedResult<CurrencyDTO>.FailureResult("Currency.SearchCurrencies", "Couldn't fetch the currency list.");
    }

}