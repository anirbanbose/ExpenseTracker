using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.Dashboard;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Dashboard.Queries;

public class SpendingChartQueryHandler : IRequestHandler<SpendingChartQuery, Result<SpendingChartDTO>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly ILogger<SpendingChartQueryHandler> _logger;

    public SpendingChartQueryHandler(IAuthenticatedUserProvider authProvider, IExpenseRepository expenseRepository, ILogger<SpendingChartQueryHandler> logger) 
    {
        _expenseRepository = expenseRepository;
        _authProvider = authProvider;
        _logger = logger;
    }

    public async Task<Result<SpendingChartDTO>> Handle(SpendingChartQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (currentUser, failureResult) = await _authProvider.GetAuthenticatedUserAsync<Result<SpendingChartDTO>>(new ResultUserNotAuthenticatedFactory<SpendingChartDTO>(), cancellationToken);
            if (failureResult != null)
                return failureResult;

            var expenses = await _expenseRepository.GetLast12MonthsExpensesAsync(currentUser!.Id, cancellationToken);
            if (expenses is not null)
            {
                var groupedExpenses = expenses
                    .GroupBy(e => new { e.ExpenseDate.Year, e.ExpenseDate.Month, Currency = $"{e.ExpenseAmount.CurrencyCode}{(!string.IsNullOrEmpty(e.ExpenseAmount.CurrencySymbol) ? " " + e.ExpenseAmount.CurrencySymbol : "")}" })
                    .Select(g => new
                    {
                        MonthYear = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM-yyyy"),
                        g.Key.Currency,
                        TotalAmount = g.Sum(x => x.ExpenseAmount.Amount)
                    })
                    .OrderBy(x => new DateTime(DateTime.ParseExact(x.MonthYear, "MMM-yyyy", null).Year,
                                   DateTime.ParseExact(x.MonthYear, "MMM-yyyy", null).Month, 1))
                    .ThenBy(x => x.Currency)
                    .ToList();
                var monthLabels = groupedExpenses
                    .Select(g => g.MonthYear)
                    .Distinct()
                    .OrderBy(d => DateTime.ParseExact(d, "MMM-yyyy", null))
                    .ToList();


                var currencies = groupedExpenses
                    .Select(g => g.Currency)
                    .Distinct()
                    .ToList();

                var datasets = currencies.Select(currency => new SpendingChartDataDTO
                {
                    Label = currency,
                    Data = monthLabels.Select(month =>
                        groupedExpenses
                            .Where(g => g.Currency == currency && g.MonthYear == month)
                            .Select(g => g.TotalAmount)
                            .FirstOrDefault() // 0 if no data
                        ).ToList()
                }).ToList();

                var spendingChartDTO = new SpendingChartDTO
                {
                    Labels = monthLabels,
                    Datasets = datasets
                };
                return Result<SpendingChartDTO>.SuccessResult(spendingChartDTO);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while fetching spending chart data for the user: {_authProvider.CurrentUserName}.");
        }
        return Result<SpendingChartDTO>.FailureResult("Chart.SpendingChart");
    }
}