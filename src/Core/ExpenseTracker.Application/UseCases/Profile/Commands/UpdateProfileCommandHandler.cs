using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Profile.Commands;

public class UpdateProfileCommandHandler : BaseHandler, IRequestHandler<UpdateProfileCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateProfileCommandHandler> _logger;

    public UpdateProfileCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<UpdateProfileCommandHandler> logger, ICurrentUserManager currentUserManager) : base(currentUserManager)
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
        try
        {
            var (currentUser, failureResult) = await GetAuthenticatedUserAsync(_userRepository, _logger, cancellationToken, new ResultUserNotAuthenticatedFactory());
            if (failureResult != null)
                return failureResult;

            var userToBeUpdated = await _userRepository.GetUserByIdAsync(currentUser.Id, cancellationToken);
            
            if(userToBeUpdated is not null)
            {
                userToBeUpdated.UpdateName(request.FirstName, request.LastName, request.MiddleName);

                _userRepository.UpdateUser(userToBeUpdated);
                await _unitOfWork.CommitAsync(cancellationToken);

                return Result.SuccessResult();
            }            
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while updating profile- {request} for the user: {CurrentUserName}.");
        }
        return Result.FailureResult("Profile.UpdateProfile");
    }
}