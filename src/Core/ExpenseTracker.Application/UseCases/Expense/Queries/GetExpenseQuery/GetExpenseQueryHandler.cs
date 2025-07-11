using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.Expense;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Expense.Queries;


public class GetExpenseQueryHandler : IRequestHandler<GetExpenseQuery, Result<ExpenseDTO>>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly IExpenseRepository _expenseRepository;
    private readonly ILogger<GetExpenseQueryHandler> _logger;

    public GetExpenseQueryHandler(IAuthenticatedUserProvider authProvider, IExpenseRepository expenseRepository, ILogger<GetExpenseQueryHandler> logger) 
    {  
        _authProvider = authProvider;
        _expenseRepository = expenseRepository;
        _logger = logger;
    }

    public async Task<Result<ExpenseDTO>> Handle(GetExpenseQuery request, CancellationToken cancellationToken)
    {
        if(request is null || !request.Id.HasValue)
        {
            return Result<ExpenseDTO>.ArgumentNullResult();
        }
        try
        {
            var (currentUser, failureResult) = await _authProvider.GetAuthenticatedUserAsync<Result<ExpenseDTO>>(new ResultUserNotAuthenticatedFactory<ExpenseDTO>(), cancellationToken);
            if (failureResult != null)
                return failureResult;

            var expense = await _expenseRepository.GetExpenseByIdAsync(request.Id.Value, currentUser.Id, cancellationToken);
            if (expense is not null)
            {
                return Result<ExpenseDTO>.SuccessResult(ExpenseDTO.FromDomain(expense));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while fetching expense details for expense id: {request.Id.Value.ToString()} for the user: {_authProvider.CurrentUserName}.");
        }
        
        return Result<ExpenseDTO>.FailureResult("Expense.GetExpenses");
    }
}
    
