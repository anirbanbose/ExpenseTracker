using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.ExpenseCategory.Commands;

public class UpdateExpenseCategoryCommandHandler : IRequestHandler<UpdateExpenseCategoryCommand, Result>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateExpenseCategoryCommandHandler> _logger;

    public UpdateExpenseCategoryCommandHandler(IAuthenticatedUserProvider authProvider, IExpenseCategoryRepository expenseCategoryRepository, IUnitOfWork unitOfWork, ILogger<UpdateExpenseCategoryCommandHandler> logger)
    {
        _authProvider = authProvider;
        _expenseCategoryRepository = expenseCategoryRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateExpenseCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result.ArgumentNullResult();
        }
        try
        {
            var (currentUser, failureResult) = await _authProvider.GetAuthenticatedUserAsync(new ResultUserNotAuthenticatedFactory(), cancellationToken);
            if (failureResult != null)
                return failureResult;

            var expenseCategory = await _expenseCategoryRepository.GetExpenseCategoryByIdAsync(request.Id, currentUser!.Id, cancellationToken);
            if (expenseCategory is null)
            {
                _logger.LogWarning($"Expense category with Id - {request.Id} not found.");
                return Result.FailureResult();
            }
            expenseCategory.UpdateName(request.Name);
            _expenseCategoryRepository.UpdateExpenseCategory(expenseCategory);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while updating expense category- {request} for the user {_authProvider.CurrentUserName}.");
        }
        return Result.FailureResult();
    }
}