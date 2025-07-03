using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Expense.Commands;

public class DeleteExpenseCommandHandler : BaseHandler, IRequestHandler<DeleteExpenseCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteExpenseCommandHandler> _logger;

    public DeleteExpenseCommandHandler(IExpenseRepository expenseRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<DeleteExpenseCommandHandler> logger, IHttpContextAccessor _httpContextAccessor) : base(_httpContextAccessor)
    {        
        _userRepository = userRepository;
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
            var (currentUser, failureResult) = await GetAuthenticatedUserAsync(_userRepository, _logger, cancellationToken, new ResultUserNotAuthenticatedFactory());
            if (failureResult != null)
                return failureResult;

            var expense = await _expenseRepository.GetExpenseByIdAsync(request.Id, currentUser.Id, cancellationToken);
            if (expense is null)
            {
                _logger.LogWarning($"Expense with Id - {request.Id} not found.");
                return Result.FailureResult("Expense.DeleteExpense", $"Expense not found.");
            }

            expense.MarkAsDeleted(); 
            _expenseRepository.UpdateExpense(expense);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while deleting expense with id {request.Id} for the user: {CurrentUserName}.");
        }
        return Result.FailureResult("Expense.DeleteExpense", "Deleting expense failed.");
    }
}