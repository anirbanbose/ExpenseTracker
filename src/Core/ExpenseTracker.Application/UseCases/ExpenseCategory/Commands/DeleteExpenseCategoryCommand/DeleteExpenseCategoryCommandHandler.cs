using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.ExpenseCategory.Commands;

public class DeleteExpenseCategoryCommandHandler : IRequestHandler<DeleteExpenseCategoryCommand, Result>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteExpenseCategoryCommandHandler> _logger;

    public DeleteExpenseCategoryCommandHandler(IAuthenticatedUserProvider authProvider, IExpenseCategoryRepository expenseCategoryRepository, IUnitOfWork unitOfWork, ILogger<DeleteExpenseCategoryCommandHandler> logger) 
    {
        _authProvider = authProvider;
        _expenseCategoryRepository = expenseCategoryRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteExpenseCategoryCommand request, CancellationToken cancellationToken)
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
                _logger.LogWarning($"Expense Category with id - {request.Id} not found.");
                return Result.FailureResult();
            }

            expenseCategory.MarkAsDeleted();
            _expenseCategoryRepository.UpdateExpenseCategory(expenseCategory);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while deleting expense category with id {request.Id} for user {_authProvider.CurrentUserName}.");
        }
        return Result.FailureResult();
    }
}