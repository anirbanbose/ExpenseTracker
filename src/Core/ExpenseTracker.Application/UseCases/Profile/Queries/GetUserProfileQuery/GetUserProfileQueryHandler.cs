using ExpenseTracker.Application.DTO.Profile;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Profile.Queries;

public class GetUserProfileQueryHandler : BaseHandler, IRequestHandler<GetUserProfileQuery, Result<UserProfileDTO>>
{

    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserProfileQueryHandler> _logger;
    public GetUserProfileQueryHandler(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, ILogger<GetUserProfileQueryHandler> logger) : base(httpContextAccessor)
    {
        _userRepository = userRepository;
        _logger = logger;        
    }

    public async Task<Result<UserProfileDTO>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
            {
                return Result<UserProfileDTO>.FailureResult("Profile.GetUserProfile", "Request cannot be null.");
            }
            if (!IsCurrentUserAuthenticated || string.IsNullOrEmpty(CurrentUserName))
            {
                return Result<UserProfileDTO>.UserNotAuthenticatedResult();
            }
            var currentUser = await _userRepository.GetUserByEmailAsync(CurrentUserName, cancellationToken);
            if (currentUser is null)
            {
                _logger.LogWarning($"User not authenticated.");
                return Result<UserProfileDTO>.UserNotAuthenticatedResult();
            }
            return Result<UserProfileDTO>.SuccessResult(UserProfileDTO.FromDomain(currentUser));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while handling GetUserProfileQuery.");
        }        
        return Result<UserProfileDTO>.FailureResult("Profile.GetUserProfile", "Couldn't fetch the profile data.");
    }
}