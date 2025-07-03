using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.ExpenseCategory.Commands;

public class DeleteExpenseCategoryCommandHandler : BaseHandler, IRequestHandler<DeleteExpenseCategoryCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteExpenseCategoryCommandHandler> _logger;

    public DeleteExpenseCategoryCommandHandler(IExpenseCategoryRepository expenseCategoryRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<DeleteExpenseCategoryCommandHandler> logger, IHttpContextAccessor _httpContextAccessor) : base(_httpContextAccessor)
    {
        _userRepository = userRepository;
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
            var (currentUser, failureResult) = await GetAuthenticatedUserAsync(_userRepository, _logger, cancellationToken, new ResultUserNotAuthenticatedFactory());
            if (failureResult != null)
                return failureResult;

            var expenseCategory = await _expenseCategoryRepository.GetExpenseCategoryByIdAsync(request.Id, currentUser.Id, cancellationToken);
            if (expenseCategory is null)
            {
                _logger.LogWarning($"Expense Category with id - {request.Id} not found.");
                return Result.FailureResult("ExpenseCategory.DeleteExpenseCategory", "Expense Category not found.");
            }

            expenseCategory.MarkAsDeleted();
            _expenseCategoryRepository.UpdateExpenseCategory(expenseCategory);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while deleting expense category with id {request.Id} for user {CurrentUserName}.");
        }
        return Result.FailureResult("Expense.DeleteExpenseCategory", "Deleting expense category failed.");
    }
}