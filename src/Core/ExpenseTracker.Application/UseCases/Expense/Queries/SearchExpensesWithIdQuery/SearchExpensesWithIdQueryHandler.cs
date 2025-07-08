using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.Expense;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Expense.Queries;

public class SearchExpensesWithIdQueryHandler : BaseHandler, IRequestHandler<SearchExpensesWithIdQuery, PagedResult<ExpenseListDTO>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SearchExpensesWithIdQueryHandler> _logger;

    public SearchExpensesWithIdQueryHandler(ICurrentUserManager currentUserManager, IExpenseRepository expenseRepository, IUserRepository userRepository, ILogger<SearchExpensesWithIdQueryHandler> logger) : base(currentUserManager)
    {
        _expenseRepository = expenseRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<PagedResult<ExpenseListDTO>> Handle(SearchExpensesWithIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (currentUser, failureResult) = await GetAuthenticatedUserAsync(_userRepository, _logger, cancellationToken, new PagedResultUserNotAuthenticatedFactory<ExpenseListDTO>());
            if (failureResult != null)
                return failureResult;

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
        return PagedResult<ExpenseListDTO>.FailureResult("Expense.SearchExpensesWithId");
    }
}