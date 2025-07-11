using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Expense.Commands;

public class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand, Result>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteExpenseCommandHandler> _logger;

    public DeleteExpenseCommandHandler(IAuthenticatedUserProvider authProvider, IExpenseRepository expenseRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<DeleteExpenseCommandHandler> logger) 
    {
        _authProvider = authProvider;
        _expenseRepository = expenseRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
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

            var expense = await _expenseRepository.GetExpenseByIdAsync(request.Id, currentUser!.Id, cancellationToken);
            if (expense is null)
            {
                _logger.LogWarning($"Expense with Id - {request.Id} not found.");
                return Result.FailureResult("Expense.DeleteExpense");
            }

            expense.MarkAsDeleted(); 
            _expenseRepository.UpdateExpense(expense);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while deleting expense with id {request.Id} for the user: {_authProvider.CurrentUserName}.");
        }
        return Result.FailureResult("Expense.DeleteExpense");
    }
}