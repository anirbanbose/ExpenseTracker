using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.ExpenseCategory.Commands;

public class UpdateExpenseCategoryCommandHandler : BaseHandler, IRequestHandler<UpdateExpenseCategoryCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateExpenseCategoryCommandHandler> _logger;

    public UpdateExpenseCategoryCommandHandler(IUserRepository userRepository, IExpenseCategoryRepository expenseCategoryRepository, IUnitOfWork unitOfWork, ILogger<UpdateExpenseCategoryCommandHandler> logger, IHttpContextAccessor _httpContextAccessor) : base(_httpContextAccessor)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _expenseCategoryRepository = expenseCategoryRepository;
    }

    public async Task<Result> Handle(UpdateExpenseCategoryCommand request, CancellationToken cancellationToken)
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
                _logger.LogWarning($"Expense category with Id - {request.Id} not found.");
                return Result.FailureResult("ExpenseCategory.UpdateExpenseCategory", "Expense category not found.");
            }
            expenseCategory.UpdateName(request.Name);
            _expenseCategoryRepository.UpdateExpenseCategory(expenseCategory);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while updating expense category- {request} for the user {CurrentUserName}.");
        }
        return Result.FailureResult("ExpenseCategory.UpdateExpenseCategory", "Update expense category failed.");
    }
}