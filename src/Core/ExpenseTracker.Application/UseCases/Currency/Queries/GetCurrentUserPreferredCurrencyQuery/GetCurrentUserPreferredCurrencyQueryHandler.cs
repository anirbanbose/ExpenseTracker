using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.Currency;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Currency.Queries;

public class GetCurrentUserPreferredCurrencyQueryHandler : IRequestHandler<GetCurrentUserPreferredCurrencyQuery, Result<CurrencyDTO>>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ILogger<GetCurrentUserPreferredCurrencyQueryHandler> _logger;

    public GetCurrentUserPreferredCurrencyQueryHandler(IAuthenticatedUserProvider authProvider, ICurrencyRepository currencyRepository, ILogger<GetCurrentUserPreferredCurrencyQueryHandler> logger) 
    {
        _currencyRepository = currencyRepository;
        _authProvider = authProvider;
        _logger = logger;
    }

    public async Task<Result<CurrencyDTO>> Handle(GetCurrentUserPreferredCurrencyQuery request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            _logger.LogWarning("GetCurrentUserPreferredCurrencyQuery request is null.");
            return Result<CurrencyDTO>.ArgumentNullResult();
        }
        try
        {
            var (currentUser, failureResult) = await _authProvider.GetAuthenticatedUserAsync<Result<CurrencyDTO>>(new ResultUserNotAuthenticatedFactory<CurrencyDTO>(), cancellationToken);
            if (failureResult != null)
                return failureResult;

            if (currentUser?.Preference is null)
            {
                _logger.LogWarning($"User preference for user {_authProvider.CurrentUserName} not found.");
                return Result<CurrencyDTO>.FailureResult();
            }
            var preferredCurrency = await _currencyRepository.GetCurrencyByIdAsync(currentUser.Preference.PreferredCurrencyId, cancellationToken);
            if (preferredCurrency is null)
            {
                _logger.LogWarning($"Preferred currency not found for user {_authProvider.CurrentUserName}.");
                return Result<CurrencyDTO>.FailureResult();
            }
            var currencyDto = CurrencyDTO.FromDomain(preferredCurrency);
            return Result<CurrencyDTO>.SuccessResult(currencyDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while handling GetCurrentUserPreferredCurrencyQuery for the user: {_authProvider.CurrentUserName}.");
        }
        return Result<CurrencyDTO>.FailureResult();

    }
}
