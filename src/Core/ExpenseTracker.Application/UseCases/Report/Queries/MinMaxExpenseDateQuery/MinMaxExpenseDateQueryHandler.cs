using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.Report;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Report.Queries;

public class MinMaxExpenseDateQueryHandler : IRequestHandler<MinMaxExpenseDateQuery, Result<MinMaxExprenseDateDTO>>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly IExpenseRepository _expenseRepository;
    private readonly ILogger<MinMaxExpenseDateQueryHandler> _logger;

    public MinMaxExpenseDateQueryHandler(IAuthenticatedUserProvider authProvider, IExpenseRepository expenseRepository, ILogger<MinMaxExpenseDateQueryHandler> logger) 
    {
        _expenseRepository = expenseRepository;
        _authProvider = authProvider;
        _logger = logger;
    }

    public async Task<Result<MinMaxExprenseDateDTO>> Handle(MinMaxExpenseDateQuery request, CancellationToken cancellationToken)
    {
        DateTime? minDate = null;
        DateTime? maxDate = null;
        try
        {
            var (currentUser, failureResult) = await _authProvider.GetAuthenticatedUserAsync<Result<MinMaxExprenseDateDTO>>(new ResultUserNotAuthenticatedFactory<MinMaxExprenseDateDTO>(), cancellationToken);
            if (failureResult != null)
                return failureResult;
            var expenses = await _expenseRepository.GetExpensesByUserIdAsync(currentUser!.Id, cancellationToken);
            if (expenses is not null && expenses.Any())
            {
                minDate = expenses.Min(e => e.ExpenseDate);
                maxDate = expenses.Max(e => e.ExpenseDate);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while handling MinMaxExpenseDateQuery for the user - {_authProvider.CurrentUserName}.");
        }
        return Result<MinMaxExprenseDateDTO>.SuccessResult(new MinMaxExprenseDateDTO(minDate, maxDate));
    }
}