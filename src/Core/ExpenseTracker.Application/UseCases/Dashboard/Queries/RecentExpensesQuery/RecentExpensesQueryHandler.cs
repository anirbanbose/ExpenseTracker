using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.Dashboard;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Dashboard.Queries;

public class RecentExpensesQueryHandler : IRequestHandler<RecentExpensesQuery, Result<List<RecentExpenseListDTO>>>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly IExpenseRepository _expenseRepository;
    private readonly ILogger<RecentExpensesQueryHandler> _logger;

    public RecentExpensesQueryHandler(IAuthenticatedUserProvider authProvider, IExpenseRepository expenseRepository, ILogger<RecentExpensesQueryHandler> logger)
    {
        _expenseRepository = expenseRepository;
        _authProvider = authProvider;
        _logger = logger;
    }

    public async Task<Result<List<RecentExpenseListDTO>>> Handle(RecentExpensesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (currentUser, failureResult) = await _authProvider.GetAuthenticatedUserAsync<Result<List<RecentExpenseListDTO>>>(new ResultUserNotAuthenticatedFactory<List<RecentExpenseListDTO>>(), cancellationToken);
            if (failureResult != null)
                return failureResult;

            var expenseList = await _expenseRepository.GetRecentExpensesAsync(currentUser!.Id, request.recordCount, cancellationToken);
            if (expenseList is not null)
            {
                List<RecentExpenseListDTO> dtoList = new List<RecentExpenseListDTO>();
                expenseList.ToList().ForEach(expense =>
                {
                    dtoList.Add(RecentExpenseListDTO.FromDomain(expense));
                });
                return Result<List<RecentExpenseListDTO>>.SuccessResult(dtoList);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while fetching recent expenses with request - {request} for the user: {_authProvider.CurrentUserName}.");
        }
        return Result<List<RecentExpenseListDTO>>.FailureResult();
    }
}
