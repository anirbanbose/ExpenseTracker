using ExpenseTracker.Application.DTO.Report;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Report.Queries;

public class MinMaxExpenseDateQueryHandler : BaseHandler, IRequestHandler<MinMaxExpenseDateQuery, Result<MinMaxExprenseDateDTO>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<MinMaxExpenseDateQueryHandler> _logger;

    public MinMaxExpenseDateQueryHandler(IHttpContextAccessor httpContextAccessor, IExpenseRepository expenseRepository, IUserRepository userRepository, ILogger<MinMaxExpenseDateQueryHandler> logger) : base(httpContextAccessor)
    {
        _expenseRepository = expenseRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<MinMaxExprenseDateDTO>> Handle(MinMaxExpenseDateQuery request, CancellationToken cancellationToken)
    {
        DateTime? minDate = null;
        DateTime? maxDate = null;
        try
        {
            var (currentUser, failureResult) = await GetAuthenticatedUserAsync(_userRepository, _logger, cancellationToken, new ResultUserNotAuthenticatedFactory<MinMaxExprenseDateDTO>());
            if (failureResult != null)
                return failureResult;
            var expenses = await _expenseRepository.GetExpensesByUserIdAsync(currentUser.Id, cancellationToken);
            if (expenses is not null && expenses.Any())
            {
                minDate = expenses.Min(e => e.ExpenseDate);
                maxDate = expenses.Max(e => e.ExpenseDate);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while handling MinMaxExpenseDateQuery for the user - {CurrentUserName}.");
        }
        return Result<MinMaxExprenseDateDTO>.SuccessResult(new MinMaxExprenseDateDTO(minDate, maxDate));
    }
}