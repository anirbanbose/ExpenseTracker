using ExpenseTracker.Application.DTO.Expense;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.Persistence.SearchModels;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Expense.Queries;

public class SearchExpensesQueryHandler : BaseHandler, IRequestHandler<SearchExpensesQuery, PagedResult<ExpenseListDTO>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SearchExpensesQueryHandler> _logger;


    public SearchExpensesQueryHandler(IHttpContextAccessor httpContextAccessor, IExpenseRepository expenseRepository, IUserRepository userRepository, ILogger<SearchExpensesQueryHandler> logger) : base(httpContextAccessor)
    {
        _expenseRepository = expenseRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<PagedResult<ExpenseListDTO>> Handle(SearchExpensesQuery request, CancellationToken cancellationToken)
    {
        if (!IsCurrentUserAuthenticated || string.IsNullOrEmpty(CurrentUserName))
        {
            return PagedResult<ExpenseListDTO>.UserNotAuthenticatedResult();
        }
        var currentUser = await _userRepository.GetUserByEmailAsync(CurrentUserName, cancellationToken);
        if (currentUser is null)
        {
            _logger.LogWarning($"User not authenticated.");
            return PagedResult<ExpenseListDTO>.UserNotAuthenticatedResult();
        }
        var expenseResult = await _expenseRepository.SearchExpensesAsync(ExpenseSearchModel.Create(request.search, request.expenseCategoryId, request.currencyId, request.startDate, request.endDate), currentUser.Id, request.pageIndex, request.pageSize, request.order, request.IsAscendingSort, cancellationToken);

        if (expenseResult.IsSuccess && expenseResult.Items is not null)
        {
            List<ExpenseListDTO> dtoList = new List<ExpenseListDTO>();
            expenseResult.Items.ToList().ForEach(expense =>
            {
                dtoList.Add(ExpenseListDTO.FromDomain(expense));
            });
            return PagedResult<ExpenseListDTO>.SuccessResult(dtoList, expenseResult.TotalCount, expenseResult.PageIndex, expenseResult.PageSize);
        }

        return PagedResult<ExpenseListDTO>.FailureResult("Expense.SearchExpenses", "Couldn't fetch the expense list.");
    }
}
