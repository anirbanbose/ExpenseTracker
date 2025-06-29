using ExpenseTracker.Application.UseCases.Dashboard;
using ExpenseTracker.Application.UseCases.Dashboard.RecentExpensesQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExpenseTracker.Presentation.Api.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ISender _sender;
        public DashboardController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("recent-expenses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RecentExpenses(int recordCount)
        {
            RecentExpensesQuery query = new RecentExpensesQuery(recordCount: recordCount);

            var expenseListResult = await _sender.Send(query);
            if (expenseListResult is not null && expenseListResult.IsSuccess)
            {
                return Ok(expenseListResult);
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, new { errorMessage = expenseListResult?.ErrorMessage ?? "Failed to retrieve expenses." });
        }


        [HttpGet("spending-chart")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SpendingChart()
        {
            var spendingChartResult = await _sender.Send(new SpendingChartQuery());
            if (spendingChartResult is not null && spendingChartResult.IsSuccess)
            {
                return Ok(spendingChartResult);
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, new { errorMessage = spendingChartResult?.ErrorMessage ?? "Failed to retrieve chart data." });
        }

        [HttpGet("expense-summary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ExpenseSummary(int yearRecordCount, int monthRecordCount, int categoryRecordCount)
        {
            var expenseSummaryResult = await _sender.Send(new ExpenseSummaryQuery(yearRecordCount, monthRecordCount, categoryRecordCount));
            if (expenseSummaryResult is not null && expenseSummaryResult.IsSuccess)
            {
                return Ok(expenseSummaryResult);
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, new { errorMessage = expenseSummaryResult?.ErrorMessage ?? "Failed to retrieve chart data." });
        }
    }
}
