using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Expense.Commands;

public class UpdateExpenseCommandHandler : BaseHandler, IRequestHandler<UpdateExpenseCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateExpenseCommandHandler> _logger;

    public UpdateExpenseCommandHandler(IExpenseRepository expenseRepository, IUserRepository userRepository, IExpenseCategoryRepository expenseCategoryRepository, ICurrencyRepository currencyRepository, IUnitOfWork unitOfWork, ILogger<UpdateExpenseCommandHandler> logger, IHttpContextAccessor _httpContextAccessor) : base(_httpContextAccessor)
    {        
        _userRepository = userRepository;
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
            var (currentUser, failureResult) = await GetAuthenticatedUserAsync(_userRepository, _logger, cancellationToken, new ResultUserNotAuthenticatedFactory());
            if (failureResult != null)
                return failureResult;

            var expense = await _expenseRepository.GetExpenseByIdAsync(request.Id, currentUser.Id, cancellationToken);
            if(expense is null)
            {
                _logger.LogWarning($"Expense with Id - {request.Id} not found.");
                return Result.FailureResult("Expense.UpdateExpense", $"Expense not found.");
            }

            var expenseCategory = await _expenseCategoryRepository.GetExpenseCategoryByIdAsync(request.CategoryId, cancellationToken);
            if (expenseCategory is null)
            {
                _logger.LogWarning($"Expense category with Id - {request.CategoryId} not found.");
                return Result.FailureResult("Expense.UpdateExpense", $"Expense category not found.");
            }
            var currency = await _currencyRepository.GetCurrencyByIdAsync(request.CurrencyId, cancellationToken);
            if (currency is null)
            {
                _logger.LogWarning($"Currency with Id {request.CurrencyId} not found.");
                return Result.FailureResult("Expense.UpdateExpense", $"Currency not found.");
            }
            expense.Update(
                new Money(request.Amount, currency.Id, currency.Code, currency.Symbol),
                request.Description,
                expenseCategory.Id,
                request.ExpenseDate.ToDate()
            );
            _expenseRepository.UpdateExpense(expense);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while updating expense- {request} for the user: {CurrentUserName}.");
        }
        return Result.FailureResult("Expense.UpdateExpense", "Update expense failed.");
    }
}
