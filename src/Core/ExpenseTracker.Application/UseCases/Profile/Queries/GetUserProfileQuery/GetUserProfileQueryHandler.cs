using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.Profile;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Profile.Queries;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserProfileDTO>>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly ILogger<GetUserProfileQueryHandler> _logger;
    public GetUserProfileQueryHandler(IAuthenticatedUserProvider authProvider, ILogger<GetUserProfileQueryHandler> logger) 
    {
        _authProvider = authProvider;
        _logger = logger;        
    }

    public async Task<Result<UserProfileDTO>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result<UserProfileDTO>.ArgumentNullResult();
        }
        try
        {
            var (currentUser, failureResult) = await _authProvider.GetAuthenticatedUserAsync<Result<UserProfileDTO>>(new ResultUserNotAuthenticatedFactory<UserProfileDTO>(), cancellationToken);
            if (failureResult != null)
                return failureResult;
            return Result<UserProfileDTO>.SuccessResult(UserProfileDTO.FromDomain(currentUser!));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while handling GetUserProfileQuery for the user: {_authProvider.CurrentUserName}.");
        }        
        return Result<UserProfileDTO>.FailureResult("Profile.GetUserProfile");
    }
}