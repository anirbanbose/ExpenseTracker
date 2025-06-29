using ExpenseTracker.Application.DTO.Currency;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Currency.Queries;

public class GetCurrentUserPreferredCurrencyQueryHandler : BaseHandler, IRequestHandler<GetCurrentUserPreferredCurrencyQuery, Result<CurrencyDTO>>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetCurrentUserPreferredCurrencyQueryHandler> _logger;


    public GetCurrentUserPreferredCurrencyQueryHandler(IHttpContextAccessor httpContextAccessor, ICurrencyRepository currencyRepository, IUserRepository userRepository, ILogger<GetCurrentUserPreferredCurrencyQueryHandler> logger) : base(httpContextAccessor)
    {
        _currencyRepository = currencyRepository;
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task<Result<CurrencyDTO>> Handle(GetCurrentUserPreferredCurrencyQuery request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            _logger.LogWarning("GetCurrentUserPreferredCurrencyQuery request is null.");
            return Result<CurrencyDTO>.ArgumentNullResult();
        }
        if (!IsCurrentUserAuthenticated || string.IsNullOrEmpty(CurrentUserName))
        {
            return Result<CurrencyDTO>.UserNotAuthenticatedResult();
        }
        try
        {            
            var currentUser = await _userRepository.GetUserByEmailAsync(CurrentUserName, cancellationToken);
            if (currentUser is null || currentUser.Deleted)
            {
                _logger.LogWarning($"User - {CurrentUserName} is not authenticated.");
                return Result<CurrencyDTO>.UserNotAuthenticatedResult();
            }
            if (currentUser.Preference is null)
            {
                _logger.LogWarning($"User preference for user {CurrentUserName} not found.");
                return Result<CurrencyDTO>.NotFoundResult("User preference not found.");
            }
            var preferredCurrency = await _currencyRepository.GetCurrencyByIdAsync(currentUser?.Preference?.PreferredCurrencyId, cancellationToken);
            if (preferredCurrency is null)
            {
                _logger.LogWarning($"Preferred currency not found for user {CurrentUserName}.");
                return Result<CurrencyDTO>.NotFoundResult("Preferred currency not found.");
            }
            var currencyDto = CurrencyDTO.FromDomain(preferredCurrency);
            return Result<CurrencyDTO>.SuccessResult(currencyDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while handling GetCurrentUserPreferredCurrencyQuery for the user: {CurrentUserName}.");
        }
        return Result<CurrencyDTO>.FailureResult("Currency.GetCurrentUserPreferredCurrency", "An error occurred while processing your request.");

    }
}
