using ExpenseTracker.Application.DTO.Dashboard;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Dashboard.Queries;

public class ExpenseSummaryQueryHandler : BaseHandler, IRequestHandler<ExpenseSummaryQuery, Result<ExpenseSummaryDTO>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ExpenseSummaryQueryHandler> _logger;

    public ExpenseSummaryQueryHandler(IHttpContextAccessor httpContextAccessor, IExpenseRepository expenseRepository, IUserRepository userRepository, ILogger<ExpenseSummaryQueryHandler> logger) : base(httpContextAccessor)
    {
        _expenseRepository = expenseRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<ExpenseSummaryDTO>> Handle(ExpenseSummaryQuery request, CancellationToken cancellationToken)
    {
        try
        {            
            var (currentUser, failureResult) = await GetAuthenticatedUserAsync(_userRepository, _logger, cancellationToken, new ResultUserNotAuthenticatedFactory<ExpenseSummaryDTO>());
            if (failureResult != null)
                return failureResult;

            var expenses = await _expenseRepository.GetExpensesByUserIdAsync(currentUser.Id, cancellationToken);

            if (expenses is not null)
            {
                DateTime today = DateTime.UtcNow;
                var startDate = new DateTime(today.Year, today.Month, 1).AddMonths(-11);

                IEnumerable<string> totalExpenses = GetYearTotalExpenses(expenses, today, startDate, request.YearTotalExpenseRecordCount);

                int currentMonth = today.Month;
                int currentYear = today.Year;

                IEnumerable<string> monthTotalExpenses = GetMonthTotalExpenses(expenses, currentMonth, currentYear, request.MonthTotalExpenseRecordCount);

                IEnumerable<CategoryExpenseDTO> categoryHighestExpenses = GetCategoryTotalExpenses(expenses, currentMonth, currentYear, request.CategoryTotalExpenseRecordCount);

                var expenseSummaryDTO = new ExpenseSummaryDTO
                {
                    TotalExpenses = totalExpenses.ToList(),
                    CurrentMonthExpenses = monthTotalExpenses.ToList(),
                    CurrentMonthTopCategoryExpense = categoryHighestExpenses.ToList(),
                };
                return Result<ExpenseSummaryDTO>.SuccessResult(expenseSummaryDTO);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while handling ExpenseSummaryQuery with request - {request}  for the user: {CurrentUserName}.");
        } 
        return Result<ExpenseSummaryDTO>.FailureResult("DashboardSummary.ExpenseSummary", "Couldn't fetch expense summary data.");
    }

    private static IEnumerable<CategoryExpenseDTO> GetCategoryTotalExpenses(IEnumerable<Domain.Models.Expense> expenses, int currentMonth, int currentYear, int recordCount)
    {
        return expenses.Where(d => d.ExpenseDate.Month == currentMonth && d.ExpenseDate.Year == currentYear)
                        .GroupBy(e => new
                        {
                            CurrencyKey = $"{e.ExpenseAmount.CurrencyCode}{(!string.IsNullOrEmpty(e.ExpenseAmount.CurrencySymbol) ? " " + e.ExpenseAmount.CurrencySymbol : "")}",
                            e.Category
                        })
                        .Select(g => new
                        {
                            Currency = g.Key.CurrencyKey,
                            g.Key.Category,
                            Total = g.Sum(x => x.ExpenseAmount.Amount)
                        })
                        .OrderByDescending(x => x.Total)
                        .Take(recordCount)
                        .Select(x => new CategoryExpenseDTO(
                            x.Category.Name,
                            $"{x.Currency}{decimal.Round(x.Total, 2, MidpointRounding.ToEven):N2}"
                        ));
    }

    private static IEnumerable<string> GetMonthTotalExpenses(IEnumerable<Domain.Models.Expense> expenses, int currentMonth, int currentYear, int recordCount)
    {
        return expenses.Where(d => d.ExpenseDate.Month == currentMonth && d.ExpenseDate.Year == currentYear)
                        .GroupBy(e => new { CurrencyKey = $"{e.ExpenseAmount.CurrencyCode}{(!string.IsNullOrEmpty(e.ExpenseAmount.CurrencySymbol) ? " " + e.ExpenseAmount.CurrencySymbol : "")}" })
                        .Select(g => new
                        {
                            Currency = g.Key.CurrencyKey,
                            Total = g.Sum(x => x.ExpenseAmount.Amount)
                        })
                        .OrderByDescending(d => d.Total)
                        .Take(recordCount)
                        .Select(g => $"{g.Currency}{decimal.Round(g.Total, 2, MidpointRounding.ToEven):N2}");
    }

    private static IEnumerable<string> GetYearTotalExpenses(IEnumerable<Domain.Models.Expense> expenses, DateTime today, DateTime startDate, int recordCount)
    {
        return expenses.Where(e => e.ExpenseDate >= startDate && e.ExpenseDate <= today)
                        .GroupBy(e => new { CurrencyKey = $"{e.ExpenseAmount.CurrencyCode}{(!string.IsNullOrEmpty(e.ExpenseAmount.CurrencySymbol) ? " " + e.ExpenseAmount.CurrencySymbol : "")}" })
                        .Select(g => new
                        {
                            Currency = g.Key.CurrencyKey,
                            Total = g.Sum(x => x.ExpenseAmount.Amount)
                        })
                        .OrderByDescending(d => d.Total)
                        .Take(recordCount)
                        .Select(d => $"{d.Currency}{decimal.Round(d.Total, 2, MidpointRounding.ToEven):N2}");
    }
}