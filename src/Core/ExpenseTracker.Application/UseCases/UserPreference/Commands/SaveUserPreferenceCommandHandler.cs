using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.UserPreference.Commands;

public class SaveUserPreferenceCommandHandler : BaseHandler, IRequestHandler<SaveUserPreferenceCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SaveUserPreferenceCommandHandler> _logger;

    public SaveUserPreferenceCommandHandler(IUserRepository userRepository, ICurrencyRepository currencyRepository, IUnitOfWork unitOfWork, ILogger<SaveUserPreferenceCommandHandler> logger, IHttpContextAccessor _httpContextAccessor) : base(_httpContextAccessor)
    {
        _userRepository = userRepository;
        _currencyRepository = currencyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(SaveUserPreferenceCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result.ArgumentNullResult();
        }
        if (!IsCurrentUserAuthenticated || string.IsNullOrEmpty(CurrentUserName))
        {
            return Result.UserNotAuthenticatedResult();
        }
        try
        {
            var currentUser = await _userRepository.GetUserByEmailAsync(CurrentUserName, cancellationToken, true);
            if (currentUser is null || currentUser.Deleted)
            {
                _logger.LogWarning($"User - {CurrentUserName} is not authenticated.");
                return Result.UserNotAuthenticatedResult();
            }
            if (currentUser.Preference is null)
            {
                _logger.LogWarning($"User preference not found for user - {CurrentUserName}.");
                return Result.FailureResult("UserPreference.SaveUserPreference", "User preference not found.");
            }
            var currency = await _currencyRepository.GetCurrencyByIdAsync(request.PreferredCurrencyId, cancellationToken);
            if (currency is null)
            {
                _logger.LogWarning($"Preferred currence with Id - {request.PreferredCurrencyId} not found for user - {CurrentUserName}.");
                return Result.FailureResult("UserPreference.SaveUserPreference", "Currency not found.");
            }
            var userPreference = currentUser.Preference;
            currentUser.AddOrUpdatePreference(currency, request.EnableMonthlyExpenseReportMail, request.EnableDailyExpenseReportMail);  
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"An error occurred while saving user preference with request: {request} for the user- {CurrentUserName}.");
        }
        return Result.FailureResult("UserPreference.SaveUserPreference", "Saving user preference failed.");
    }
}