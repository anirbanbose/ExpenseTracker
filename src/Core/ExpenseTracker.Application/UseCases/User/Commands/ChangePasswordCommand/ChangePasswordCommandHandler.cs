﻿using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.User.Commands;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(IAuthenticatedUserProvider authProvider, IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<ChangePasswordCommandHandler> logger) 
    {
        _authProvider = authProvider;
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
            var (currentUser, failureResult) = await _authProvider.GetAuthenticatedUserAsync<Result>(new ResultUserNotAuthenticatedFactory(), cancellationToken);
            if (failureResult != null)
                return failureResult;

            var userToBeUpdated = await _userRepository.GetUserByIdAsync(currentUser!.Id, cancellationToken);
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
                return Result.FailureResult("Account.ChangePassword");
            }            
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while updating user password for the user: {_authProvider.CurrentUserName} with password change request - {request}.");
        }
        return Result.FailureResult("Account.ChangePassword");
    }
}