using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.SharedKernel.Results;
using ExpenseTracker.Domain.Utils;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Expense.Commands;

public class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, Result>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly IExpenseRepository _expenseRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateExpenseCommandHandler> _logger;

    public UpdateExpenseCommandHandler(IAuthenticatedUserProvider authProvider, IExpenseRepository expenseRepository, IExpenseCategoryRepository expenseCategoryRepository, ICurrencyRepository currencyRepository, IUnitOfWork unitOfWork, ILogger<UpdateExpenseCommandHandler> logger) 
    {
        _authProvider = authProvider;
        _expenseRepository = expenseRepository;
        _currencyRepository = currencyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _expenseCategoryRepository = expenseCategoryRepository;
    }

    public async Task<Result> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
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

            var userPreference = currentUser!.Preference;
            if (userPreference is null)
            {
                _logger.LogWarning($"User preference not found for user: {_authProvider.CurrentUserName}.");
                return Result<Guid?>.FailureResult();
            }

            var expense = await _expenseRepository.GetExpenseByIdAsync(request.Id, currentUser!.Id, cancellationToken);
            if(expense is null)
            {
                _logger.LogWarning($"Expense with Id - {request.Id} not found.");
                return Result.FailureResult();
            }

            var expenseCategory = await _expenseCategoryRepository.GetExpenseCategoryByIdAsync(request.CategoryId, cancellationToken);
            if (expenseCategory is null)
            {
                _logger.LogWarning($"Expense category with Id - {request.CategoryId} not found.");
                return Result.FailureResult();
            }
            var currency = await _currencyRepository.GetCurrencyByIdAsync(userPreference.PreferredCurrencyId, cancellationToken);
            if (currency is null)
            {
                _logger.LogWarning($"Currency with Id {userPreference.PreferredCurrencyId} not found.");
                return Result<Guid?>.FailureResult();
            }

            expense.Update(
                new Money(request.Amount, currency.Code, currency.Symbol),
                request.Description,
                expenseCategory,
                request.ExpenseDate.ToDate()
            );
            _expenseRepository.UpdateExpense(expense);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while updating expense- {request} for the user: {_authProvider.CurrentUserName}.");
        }
        return Result.FailureResult();
    }
}
