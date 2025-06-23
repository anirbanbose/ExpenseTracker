using ExpenseTracker.Application.UseCases.Dashboard.RecentExpensesQuery;
using ExpenseTracker.Application.UseCases.Expense.Commands;
using ExpenseTracker.Application.UseCases.Expense.Queries;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Presentation.Api.Controllers;


[Authorize]
[Route("api/expense")]
[ApiController]
public class ExpenseController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<ExpenseController> _logger;

    public ExpenseController(ILogger<ExpenseController> logger, ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    [HttpPost("add-new")]
    [ProducesResponseType(StatusCodes.Status200OK)] 
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddNewExpense([FromBody] AddNewExpenseCommand request)
    {
        if (request == null)
        {
            return BadRequest(new { errorMessage = "Request cannot be null" });
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized(new { errorMessage = "User is not authenticated" });
        }
        var result = await _sender.Send(request);

        if (result.IsSuccess)
        {
            return Ok(new { message = "Expense added successfully" });
        }
        return BadRequest(new { errorMessage = result.ErrorMessage ?? "Failed to add expense" });
    }

    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchExpenses(string? search, string? categoryId, string? currencyId, string? startDate, string? endDate, int pageIndex = 0, int pageSize = 10, int order = 2, bool isAscending = false)
    {
        SearchExpensesQuery query = new SearchExpensesQuery(
            search: search,
            expenseCategoryId: categoryId.IsGuid() ? categoryId!.ToGuid() : null,
            currencyId: currencyId.IsGuid() ? currencyId!.ToGuid() : null,
            startDate: startDate.IsDate() ? startDate!.ToDate() : null,
            endDate: endDate.IsDate() ? endDate!.ToDate() : null,
            pageIndex: pageIndex,
            pageSize: pageSize,
            order: (ExpenseListOrder)order,
            IsAscendingSort: isAscending
        );
        
        var expenseListResult = await _sender.Send(query);
        if (expenseListResult.IsSuccess)
        {
            return Ok(expenseListResult);
        }
        return BadRequest(new { errorMessage = expenseListResult.ErrorMessage ?? "Failed to retrieve expenses." });
    }

    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetExpense(string? id)
    {
        GetExpenseQuery query = new GetExpenseQuery(Id : id.IsGuid() ? id!.ToGuid() : null);

        var expenseResult = await _sender.Send(query);
        if (expenseResult.IsSuccess)
        {
            return Ok(expenseResult);
        }
        return BadRequest(new { errorMessage = expenseResult.ErrorMessage ?? "Failed to retrieve expense." });
    }

    [HttpDelete()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteExpense(string? id)
    {
        if (id is null || string.IsNullOrEmpty(id) || !id.IsGuid())
        {
            return BadRequest(new { errorMessage = "Not a valid Id." });
        }
        DeleteExpenseCommand command = new DeleteExpenseCommand()
        {
            Id = id!.ToGuid().Value
        };

        var expenseDeleteResult = await _sender.Send(command);
        if (expenseDeleteResult.IsSuccess)
        {
            return Ok();
        }
        return BadRequest(new { errorMessage = expenseDeleteResult.ErrorMessage ?? "Failed to delete expense." });
    }


    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateExpense([FromBody] UpdateExpenseCommand request)
    {
        if (request == null)
        {
            return BadRequest(new { errorMessage = "Request cannot be null" });
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized(new { errorMessage = "User is not authenticated" });
        }
        var result = await _sender.Send(request);

        if (result.IsSuccess)
        {
            return Ok(new { message = "Expense updated successfully" });
        }
        return BadRequest(new { errorMessage = result.ErrorMessage ?? "Failed to update expense" });
    }


}
