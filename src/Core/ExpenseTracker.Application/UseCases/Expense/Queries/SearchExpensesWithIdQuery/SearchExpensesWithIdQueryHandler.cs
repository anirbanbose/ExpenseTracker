using ExpenseTracker.Application.DTO.Expense;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Expense.Queries;

public class SearchExpensesWithIdQueryHandler : BaseHandler, IRequestHandler<SearchExpensesWithIdQuery, PagedResult<ExpenseListDTO>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SearchExpensesWithIdQueryHandler> _logger;

    public SearchExpensesWithIdQueryHandler(IHttpContextAccessor httpContextAccessor, IExpenseRepository expenseRepository, IUserRepository userRepository, ILogger<SearchExpensesWithIdQueryHandler> logger) : base(httpContextAccessor)
    {
        _expenseRepository = expenseRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<PagedResult<ExpenseListDTO>> Handle(SearchExpensesWithIdQuery request, CancellationToken cancellationToken)
    {
        if (!IsCurrentUserAuthenticated || string.IsNullOrEmpty(CurrentUserName))
        {
            return PagedResult<ExpenseListDTO>.UserNotAuthenticatedResult();
        }
        try
        {
            var currentUser = await _userRepository.GetUserByEmailAsync(CurrentUserName, cancellationToken);
            if (currentUser is null || currentUser.Deleted)
            {
                _logger.LogWarning($"User - {CurrentUserName} is not authenticated.");
                return PagedResult<ExpenseListDTO>.UserNotAuthenticatedResult();
            }
            var expenseResult = await _expenseRepository.SearchExpensesAsync(request.id, currentUser.Id, request.pageSize, cancellationToken);

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
            _logger.LogError(ex, $"Error occurred while searching expenses for the user: {CurrentUserName}.");
        }
        return PagedResult<ExpenseListDTO>.FailureResult("Expense.SearchExpenses", "Couldn't fetch the expense list.");
    }
}