using ExpenseTracker.Application.DTO.ExpenseCategory;
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
            if (!IsCurrentUserAuthenticated || string.IsNullOrEmpty(CurrentUserName))
            {
                return Result<ExpenseCategoryDTO?>.UserNotAuthenticatedResult();
            }
            var currentUser = await _userRepository.GetUserByEmailAsync(CurrentUserName, cancellationToken);
            if (currentUser is null)
            {
                _logger.LogWarning($"User not authenticated.");
                return Result.UserNotAuthenticatedResult();
            }
            var expense = await _expenseRepository.GetExpenseByIdAsync(request.Id, currentUser.Id, cancellationToken);
            if (expense is null)
            {
                return Result.FailureResult("Expense.DeleteExpense", "Expense not found.");
            }

            expense.MarkAsDeleted(); 
            _expenseRepository.UpdateExpense(expense);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex.Message, ex);
        }
        return Result.FailureResult("Expense.DeleteExpense", "Deleting expense failed.");
    }
}