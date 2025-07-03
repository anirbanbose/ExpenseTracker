using ExpenseTracker.Application.DTO.Dashboard;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Dashboard.Queries;

public class RecentExpensesQueryHandler : BaseHandler, IRequestHandler<RecentExpensesQuery, Result<List<RecentExpenseListDTO>>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<RecentExpensesQueryHandler> _logger;


    public RecentExpensesQueryHandler(IHttpContextAccessor httpContextAccessor, IExpenseRepository expenseRepository, IUserRepository userRepository, ILogger<RecentExpensesQueryHandler> logger) : base(httpContextAccessor)
    {
        _expenseRepository = expenseRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<List<RecentExpenseListDTO>>> Handle(RecentExpensesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (currentUser, failureResult) = await GetAuthenticatedUserAsync(_userRepository, _logger, cancellationToken, new ResultUserNotAuthenticatedFactory<List<RecentExpenseListDTO>>());
            if (failureResult != null)
                return failureResult;

            var expenseResult = await _expenseRepository.GetRecentExpensesAsync(currentUser.Id, request.recordCount, cancellationToken);
            if (expenseResult is not null)
            {
                List<RecentExpenseListDTO> dtoList = new List<RecentExpenseListDTO>();
                expenseResult.ToList().ForEach(expense =>
                {
                    dtoList.Add(RecentExpenseListDTO.FromDomain(expense));
                });
                return Result<List<RecentExpenseListDTO>>.SuccessResult(dtoList);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while fetching recent expenses with request - {request} for the user: {CurrentUserName}.");
        }
        return Result<List<RecentExpenseListDTO>>.FailureResult("Expense.RecentExpenses", "Couldn't fetch the expense list.");
    }
}
