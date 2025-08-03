using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.UserPreference.Commands;

public class SaveUserPreferenceCommandHandler : IRequestHandler<SaveUserPreferenceCommand, Result>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SaveUserPreferenceCommandHandler> _logger;

    public SaveUserPreferenceCommandHandler(IAuthenticatedUserProvider authProvider, IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<SaveUserPreferenceCommandHandler> logger) 
    {
        _authProvider = authProvider;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(SaveUserPreferenceCommand request, CancellationToken cancellationToken)
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
            if (userToBeUpdated is null || userToBeUpdated.Preference is null)
            {
                _logger.LogWarning($"User preference not found for user - {_authProvider.CurrentUserName}.");
                return Result.FailureResult();
            }
            var userPreference = userToBeUpdated.Preference;
            userPreference.Update(request.EnableMonthlyExpenseReportMail, request.EnableDailyExpenseReportMail);
            _userRepository.UpdateUser(userToBeUpdated);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"An error occurred while saving user preference with request: {request} for the user- {_authProvider.CurrentUserName}.");
        }
        return Result.FailureResult();
    }
}