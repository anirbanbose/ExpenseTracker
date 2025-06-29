using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Profile.Commands;


public class UpdateProfileCommandHandler : BaseHandler, IRequestHandler<UpdateProfileCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateProfileCommandHandler> _logger;

    public UpdateProfileCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<UpdateProfileCommandHandler> logger, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;        
    }

    public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
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

            currentUser.UpdateName(request.FirstName, request.LastName, request.MiddleName);            

            _userRepository.UpdateUser(currentUser);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while updating profile- {request} for the user: {CurrentUserName}.");
        }
        return Result.FailureResult("Profile.UpdateProfile", "Profile update failed.");
    }
}