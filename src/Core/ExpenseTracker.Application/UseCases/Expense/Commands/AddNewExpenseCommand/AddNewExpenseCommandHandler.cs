using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Expense.Commands;

public class AddNewExpenseCommandHandler : BaseHandler, IRequestHandler<AddNewExpenseCommand, Result<Guid?>>
{
    private readonly IUserRepository _userRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddNewExpenseCommandHandler> _logger;

    public AddNewExpenseCommandHandler(IExpenseRepository expenseRepository, IUserRepository userRepository, IExpenseCategoryRepository expenseCategoryRepository, ICurrencyRepository currencyRepository, IUnitOfWork unitOfWork, ILogger<AddNewExpenseCommandHandler> logger, IHttpContextAccessor _httpContextAccessor) : base(_httpContextAccessor)
    {
        _userRepository = userRepository;
        _expenseCategoryRepository = expenseCategoryRepository;
        _currencyRepository = currencyRepository;
        _expenseRepository = expenseRepository;
        _unitOfWork = unitOfWork;
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
            var (currentUser, failureResult) = await GetAuthenticatedUserAsync(_userRepository, _logger, cancellationToken, new ResultUserNotAuthenticatedFactory<Guid?>());
            if (failureResult != null)
                return failureResult;

            var userPreference = currentUser?.Preference;
            if (userPreference is null)
            {
                _logger.LogWarning($"User preference not found for user: {CurrentUserName}.");
                return Result<Guid?>.FailureResult("Expense.AddNewExpense", $"Adding new expense failed.");
            }

            var expenseCategory = await _expenseCategoryRepository.GetExpenseCategoryByIdAsync(request.CategoryId, cancellationToken);
            if (expenseCategory is null)
            {
                _logger.LogWarning($"Expense category with Id - {request.CategoryId} not found.");
                return Result<Guid?>.FailureResult("Expense.AddNewExpense", $"Expense category not found.");
            }
            var currency = await _currencyRepository.GetCurrencyByIdAsync(userPreference.PreferredCurrencyId, cancellationToken);
            if (currency is null)
            {
                _logger.LogWarning($"Currency with Id {userPreference.PreferredCurrencyId} not found.");
                return Result<Guid?>.FailureResult("Expense.AddNewExpense", $"Currency not found.");
            }
            var expense = new Domain.Models.Expense(
                    new Money(request.Amount, currency.Code, currency.Symbol), 
                    request.Description, 
                    expenseCategory.Id, 
                    request.ExpenseDate.ToDate(), 
                    currentUser.Id
                    );
            
            await _expenseRepository.AddExpenseAsync(expense);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Guid?>.SuccessResult(expense.Id);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while adding new expense- {request} for the user: {CurrentUserName}.");
        }
        return Result<Guid?>.FailureResult("Expense.AddNewExpense", "Adding new expense failed.");
    }
}
