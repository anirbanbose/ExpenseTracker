using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.ExpenseCategory.Commands;

public class AddNewExpenseCategoryCommandHandler : IRequestHandler<AddNewExpenseCategoryCommand, Result<Guid?>>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddNewExpenseCategoryCommandHandler> _logger;

    public AddNewExpenseCategoryCommandHandler(IAuthenticatedUserProvider authProvider, IExpenseCategoryRepository expenseCategoryRepository, IUnitOfWork unitOfWork, ILogger<AddNewExpenseCategoryCommandHandler> logger) 
    {
        _authProvider = authProvider;
        _expenseCategoryRepository = expenseCategoryRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid?>> Handle(AddNewExpenseCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result<Guid?>.ArgumentNullResult();
        }
        try
        {
            var (currentUser, failureResult) = await _authProvider.GetAuthenticatedUserAsync(new ResultUserNotAuthenticatedFactory<Guid?>(), cancellationToken);
            if (failureResult != null)
                return failureResult;

            var expenseCategory = new Domain.Models.ExpenseCategory(request.Name, false, currentUser!.Id);

            await _expenseCategoryRepository.AddExpenseCategoryAsync(expenseCategory);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Guid?>.SuccessResult(expenseCategory.Id);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while adding new expense category- {request} for user - {_authProvider.CurrentUserName}.");
        }
        return Result<Guid?>.FailureResult("Expense.AddNewExpenseCategory");
    }
}