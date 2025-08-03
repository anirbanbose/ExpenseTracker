using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.Expense;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Expense.Queries;

public class SearchExpensesWithIdQueryHandler : IRequestHandler<SearchExpensesWithIdQuery, PagedResult<ExpenseListDTO>>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly IExpenseRepository _expenseRepository;
    private readonly ILogger<SearchExpensesWithIdQueryHandler> _logger;

    public SearchExpensesWithIdQueryHandler(IAuthenticatedUserProvider authProvider, IExpenseRepository expenseRepository, ILogger<SearchExpensesWithIdQueryHandler> logger) 
    {
        _expenseRepository = expenseRepository;
        _authProvider = authProvider;
        _logger = logger;
    }

    public async Task<PagedResult<ExpenseListDTO>> Handle(SearchExpensesWithIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (currentUser, failureResult) = await _authProvider.GetAuthenticatedUserAsync<PagedResult<ExpenseListDTO>>(new PagedResultUserNotAuthenticatedFactory<ExpenseListDTO>(), cancellationToken);
            if (failureResult != null)
                return failureResult;

            var expenseResult = await _expenseRepository.SearchExpensesAsync(request.id, currentUser!.Id, request.pageSize, cancellationToken);
            if (expenseResult.Items is null)
            {
                return PagedResult<ExpenseListDTO>.FailureResult();
            }
            
            List<ExpenseListDTO> dtoList = new List<ExpenseListDTO>();
            expenseResult.Items.ToList().ForEach(expense =>
            {
                dtoList.Add(ExpenseListDTO.FromDomain(expense));
            });
            return PagedResult<ExpenseListDTO>.SuccessResult(dtoList, expenseResult.TotalCount, expenseResult.PageIndex, request.pageSize);
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while searching expenses for the user: {_authProvider.CurrentUserName}.");
        }
        return PagedResult<ExpenseListDTO>.FailureResult();
    }
}