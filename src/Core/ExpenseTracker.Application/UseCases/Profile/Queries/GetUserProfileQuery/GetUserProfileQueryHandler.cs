using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.Profile;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Profile.Queries;

public class GetUserProfileQueryHandler : BaseHandler, IRequestHandler<GetUserProfileQuery, Result<UserProfileDTO>>
{

    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserProfileQueryHandler> _logger;
    public GetUserProfileQueryHandler(ICurrentUserManager currentUserManager, IUserRepository userRepository, ILogger<GetUserProfileQueryHandler> logger) : base(currentUserManager)
    {
        _userRepository = userRepository;
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
            var (currentUser, failureResult) = await GetAuthenticatedUserAsync(_userRepository, _logger, cancellationToken, new ResultUserNotAuthenticatedFactory<UserProfileDTO>());
            if (failureResult != null)
                return failureResult;
            return Result<UserProfileDTO>.SuccessResult(UserProfileDTO.FromDomain(currentUser));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while handling GetUserProfileQuery for the user: {CurrentUserName}.");
        }        
        return Result<UserProfileDTO>.FailureResult("Profile.GetUserProfile");
    }
}