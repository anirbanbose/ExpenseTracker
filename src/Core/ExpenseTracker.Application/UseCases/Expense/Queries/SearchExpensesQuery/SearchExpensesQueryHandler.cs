using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.Expense;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.Persistence.SearchModels;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Expense.Queries;

public class SearchExpensesQueryHandler : IRequestHandler<SearchExpensesQuery, PagedResult<ExpenseListDTO>>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly IExpenseRepository _expenseRepository;
    private readonly ILogger<SearchExpensesQueryHandler> _logger;

    public SearchExpensesQueryHandler(IAuthenticatedUserProvider authProvider, IExpenseRepository expenseRepository, ILogger<SearchExpensesQueryHandler> logger) 
    {
        _expenseRepository = expenseRepository;
        _authProvider = authProvider;
        _logger = logger;
    }

    public async Task<PagedResult<ExpenseListDTO>> Handle(SearchExpensesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (currentUser, failureResult) = await _authProvider.GetAuthenticatedUserAsync<PagedResult<ExpenseListDTO>>(new PagedResultUserNotAuthenticatedFactory<ExpenseListDTO>(), cancellationToken);
            if (failureResult != null)
                return failureResult;

            var expenseResult = await _expenseRepository.SearchExpensesAsync(ExpenseSearchModel.Create(request.search, request.expenseCategoryId, request.startDate, request.endDate), currentUser!.Id, request.pageIndex, request.pageSize, request.order, request.IsAscendingSort, cancellationToken);
            if (expenseResult.IsSuccess && expenseResult.Items is not null)
            {
                List<ExpenseListDTO> dtoList = new List<ExpenseListDTO>();
                expenseResult.Items.ToList().ForEach(expense =>
                {
                    dtoList.Add(ExpenseListDTO.FromDomain(expense));
                });
                return PagedResult<ExpenseListDTO>.SuccessResult(dtoList, expenseResult.TotalCount, expenseResult.PageIndex, expenseResult.PageSize);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while searching expenses with request - {request} for the user: {_authProvider.CurrentUserName}.");
        }      

        return PagedResult<ExpenseListDTO>.FailureResult("Expense.SearchExpenses");
    }
}
