using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Expense.Commands;

public class AddNewExpenseCommandHandler : BaseHandler, IRequestHandler<AddNewExpenseCommand, Result>
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

    public async Task<Result> Handle(AddNewExpenseCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result.ArgumentNullResult();
        }
        try
        {
            if (!IsCurrentUserAuthenticated || string.IsNullOrEmpty(CurrentUserName))
            {
                return Result.UserNotAuthenticatedResult();
            }
            var currentUser = await _userRepository.GetUserByEmailAsync(CurrentUserName, cancellationToken);
            if (currentUser is null)
            {
                _logger.LogWarning($"User not authenticated.");
                return Result.UserNotAuthenticatedResult();
            }            
            var expenseCategory = await _expenseCategoryRepository.GetExpenseCategoryByIdAsync(request.CategoryId, cancellationToken);
            if (expenseCategory is null)
            {
                return Result.FailureResult("Expense.AddNewExpense", "Expense category not found.");
            }
            var currency = await _currencyRepository.GetCurrencyByIdAsync(request.CurrencyId, cancellationToken);
            if (currency is null)
            {
                return Result.FailureResult("Expense.AddNewExpense", "Currency not found.");
            }
            var expense = new Domain.Models.Expense(
                    new Money(request.Amount, currency.Id, currency.Code, currency.Symbol), 
                    request.Description, 
                    expenseCategory.Id, 
                    request.ExpenseDate.ToDate(), 
                    currentUser.Id
                    );
            
            await _expenseRepository.AddExpenseAsync(expense);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex.Message, ex);
        }
        return Result.FailureResult("Expense.AddNewExpense", "Adding new expense failed.");
    }
}
