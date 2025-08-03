using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.Dashboard;
using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Profile.Commands;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateProfileCommandHandler> _logger;

    public UpdateProfileCommandHandler(IAuthenticatedUserProvider authProvider, IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<UpdateProfileCommandHandler> logger) 
    {
        _authProvider = authProvider;
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
            var (currentUser, failureResult) = await _authProvider.GetAuthenticatedUserAsync<Result>(new ResultUserNotAuthenticatedFactory(), cancellationToken);
            if (failureResult != null)
                return failureResult;

            var userToBeUpdated = await _userRepository.GetUserByIdAsync(currentUser!.Id, cancellationToken);
            
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
            _logger?.LogError(ex, $"Error occurred while updating profile- {request} for the user: {_authProvider.CurrentUserName}.");
        }
        return Result.FailureResult();
    }
}