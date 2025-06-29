using ExpenseTracker.Application.DTO.UserPreference;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.UserPreference.Queries;

public class GetUserPreferenceQueryHandler : BaseHandler, IRequestHandler<GetUserPreferenceQuery, Result<UserPreferenceDTO>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserPreferenceQueryHandler> _logger;

    public GetUserPreferenceQueryHandler(IHttpContextAccessor httpContextAccessor, IExpenseRepository expenseRepository, IUserRepository userRepository, ILogger<GetUserPreferenceQueryHandler> logger) : base(httpContextAccessor)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<UserPreferenceDTO>> Handle(GetUserPreferenceQuery request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result<UserPreferenceDTO>.ArgumentNullResult();
        }
        if (!IsCurrentUserAuthenticated || string.IsNullOrEmpty(CurrentUserName))
        {
            return Result<UserPreferenceDTO>.UserNotAuthenticatedResult();
        }
        try
        {
            var currentUser = await _userRepository.GetUserByEmailAsync(CurrentUserName, cancellationToken);
            if (currentUser is null || currentUser.Deleted)
            {
                _logger.LogWarning($"User - {CurrentUserName} is not authenticated.");
                return Result<UserPreferenceDTO>.UserNotAuthenticatedResult();
            }
            if (currentUser.Preference is null)
            {
                _logger.LogWarning($"User preference not found for user: {CurrentUserName}");
                return Result<UserPreferenceDTO>.NotFoundResult("User preference not found.");
            }
            return Result<UserPreferenceDTO>.SuccessResult(UserPreferenceDTO.FromDomain(currentUser.Preference));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while handling GetUserPreferenceQuery for the user - {CurrentUserName}.");
        }        
        return Result<UserPreferenceDTO>.FailureResult("UserPreference.GetUserPreference", "Couldn't fetch user preference data.");
    }
}