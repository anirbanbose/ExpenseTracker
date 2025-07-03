using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.User.Commands;

public class ChangePasswordCommandHandler : BaseHandler, IRequestHandler<ChangePasswordCommand, Result>
{

    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<ChangePasswordCommandHandler> logger, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
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
                var verifyPasswordResult = currentUser.VerifyPassword(request.CurrentPassword);
                if (verifyPasswordResult.IsSuccess)
                {
                    userToBeUpdated.ChangePassword(request.NewPassword);
                    _userRepository.UpdateUser(userToBeUpdated);
                    await _unitOfWork.CommitAsync(cancellationToken);
                    return Result.SuccessResult();
                }
                return Result.FailureResult("Account.ChangePassword", "Password couldn't be changed. There is an issue with the Password combination.");
            }            
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while updating user password for the user: {CurrentUserName} with password change request - {request}.");
        }
        return Result.FailureResult("Account.ChangePassword", "Password couldn't be changed. Please try again later.");
    }
}