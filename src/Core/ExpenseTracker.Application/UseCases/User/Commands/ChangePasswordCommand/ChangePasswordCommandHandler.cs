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
            var verifyPasswordResult = currentUser.VerifyPassword(request.CurrentPassword);
            if (verifyPasswordResult.IsSuccess)
            {
                currentUser.ChangePassword(request.NewPassword);
                _userRepository.UpdateUser(currentUser);
                await _unitOfWork.CommitAsync(cancellationToken);
                return Result.SuccessResult();
            }
            return Result.FailureResult("Account.ChangePassword", "Password couldn't be changed. There is an issue with the Password combination.");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while updating user password for the user: {CurrentUserName} with password change request - {request}.");
        }
        return Result.FailureResult("Account.ChangePassword", "Password couldn't be changed. Please try again later.");
    }
}