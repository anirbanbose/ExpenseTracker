using ExpenseTracker.Application.DTO.Expense;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Expense.Queries;


public class GetExpenseQueryHandler : BaseHandler, IRequestHandler<GetExpenseQuery, Result<ExpenseDTO>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetExpenseQueryHandler> _logger;

    public GetExpenseQueryHandler(IHttpContextAccessor httpContextAccessor, IExpenseRepository expenseRepository, IUserRepository userRepository, ILogger<GetExpenseQueryHandler> logger) : base(httpContextAccessor)
    {        
        _expenseRepository = expenseRepository;
        _userRepository = userRepository;
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
            var (currentUser, failureResult) = await GetAuthenticatedUserAsync(_userRepository, _logger, cancellationToken, new ResultUserNotAuthenticatedFactory<ExpenseDTO>());
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
            _logger.LogError(ex, $"Error occurred while fetching expense details for expense id: {request.Id.Value.ToString()} for the user: {CurrentUserName}.");
        }
        
        return Result<ExpenseDTO>.FailureResult("Expense.SearchExpenses", "Couldn't fetch the expense list.");
    }
}
    
