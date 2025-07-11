using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.UserPreference;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.UserPreference.Queries;

public class GetUserPreferenceQueryHandler : IRequestHandler<GetUserPreferenceQuery, Result<UserPreferenceDTO>>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly ILogger<GetUserPreferenceQueryHandler> _logger;

    public GetUserPreferenceQueryHandler(IAuthenticatedUserProvider authProvider, IExpenseRepository expenseRepository, ILogger<GetUserPreferenceQueryHandler> logger) 
    {
        _authProvider = authProvider;
        _logger = logger;
    }

    public async Task<Result<UserPreferenceDTO>> Handle(GetUserPreferenceQuery request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result<UserPreferenceDTO>.ArgumentNullResult();
        }
        try
        {
            var (currentUser, failureResult) = await _authProvider.GetAuthenticatedUserAsync<Result<UserPreferenceDTO>>(new ResultUserNotAuthenticatedFactory<UserPreferenceDTO>(), cancellationToken);
            if (failureResult != null)
                return failureResult;

            if(currentUser?.Preference is null)
            {
                _logger.LogWarning($"User preference not found for user: {_authProvider.CurrentUserName}");
                return Result<UserPreferenceDTO>.NotFoundResult();
            }
            return Result<UserPreferenceDTO>.SuccessResult(UserPreferenceDTO.FromDomain(currentUser!.Preference));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while handling GetUserPreferenceQuery for the user - {_authProvider.CurrentUserName}.");
        }        
        return Result<UserPreferenceDTO>.FailureResult("UserPreference.GetUserPreference");
    }
}