using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.SharedKernel.Results;
using ExpenseTracker.Domain.Utils;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Expense.Commands;

public class AddNewExpenseCommandHandler : IRequestHandler<AddNewExpenseCommand, Result<Guid?>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly ILogger<AddNewExpenseCommandHandler> _logger;

    public AddNewExpenseCommandHandler(IAuthenticatedUserProvider authProvider, IExpenseRepository expenseRepository, IExpenseCategoryRepository expenseCategoryRepository, ICurrencyRepository currencyRepository, IUnitOfWork unitOfWork, ILogger<AddNewExpenseCommandHandler> logger)
    {
        _expenseCategoryRepository = expenseCategoryRepository;
        _currencyRepository = currencyRepository;
        _expenseRepository = expenseRepository;
        _unitOfWork = unitOfWork;
        _authProvider = authProvider;
        _logger = logger;
    }

    public async Task<Result<Guid?>> Handle(AddNewExpenseCommand request, CancellationToken cancellationToken)
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

            var userPreference = currentUser?.Preference;
            if (userPreference is null)
            {
                _logger.LogWarning($"User preference not found for user: {_authProvider.CurrentUserName}.");
                return Result<Guid?>.FailureResult();
            }

            var expenseCategory = await _expenseCategoryRepository.GetExpenseCategoryByIdAsync(request.CategoryId, cancellationToken);
            if (expenseCategory is null)
            {
                _logger.LogWarning($"Expense category with Id - {request.CategoryId} not found.");
                return Result<Guid?>.FailureResult();
            }
            var currency = await _currencyRepository.GetCurrencyByIdAsync(userPreference.PreferredCurrencyId, cancellationToken);
            if (currency is null)
            {
                _logger.LogWarning($"Currency with Id {userPreference.PreferredCurrencyId} not found.");
                return Result<Guid?>.FailureResult();
            }
            var expense = new Domain.Models.Expense(
                    new Money(request.Amount, currency.Code, currency.Symbol), 
                    request.Description, 
                    expenseCategory, 
                    request.ExpenseDate.ToDate(), 
                    currentUser!.Id
                    );
            
            await _expenseRepository.AddExpenseAsync(expense);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Guid?>.SuccessResult(expense.Id);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while adding new expense- {request} for the user: {_authProvider.CurrentUserName}.");
        }
        return Result<Guid?>.FailureResult();
    }
}
