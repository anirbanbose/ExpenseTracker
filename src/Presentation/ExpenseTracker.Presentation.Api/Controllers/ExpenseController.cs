using ExpenseTracker.Application.DTO.Expense;
using ExpenseTracker.Application.UseCases.Expense.Commands;
using ExpenseTracker.Application.UseCases.Expense.Queries;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.SharedKernel.Results;
using ExpenseTracker.Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
            _logger.LogInformation("Add expense request is null.");
            return BadRequest(new { errorMessage = "Request cannot be null" });
        }
        if (!ModelState.IsValid)
        {
            _logger.LogInformation("Model state is invalid: {ModelState}", ModelState);
            return BadRequest(ModelState);
        }
        var result = await _sender.Send(request);

        if (result is not null && result.IsSuccess)
        {
            return Ok(new { id = result.Value, message = "Expense added successfully" });
        }
        return StatusCode((int)HttpStatusCode.InternalServerError, new { errorMessage = result?.ErrorMessage ?? "Failed to add expense" });
    }

    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SearchExpenses(string? search, string? categoryId, string? startDate, string? endDate, int pageIndex = 0, int pageSize = 10, ExpenseListOrder order = ExpenseListOrder.ExpenseDate, bool isAscending = false)
    {        
        SearchExpensesQuery query = new SearchExpensesQuery(
                search: search,
                expenseCategoryId: categoryId.IsGuid() ? categoryId!.ToGuid() : null,
                startDate: startDate.IsDate() ? startDate!.ToDate() : null,
                endDate: endDate.IsDate() ? endDate!.ToDate() : null,
                pageIndex: pageIndex,
                pageSize: pageSize,
                order: order,
                IsAscendingSort: isAscending
            );
        PagedResult<ExpenseListDTO> expenseListResult = await _sender.Send(query);

        if (expenseListResult is not null && expenseListResult.IsSuccess)
        {
            return Ok(expenseListResult);
        }
        return BadRequest(new { errorMessage = expenseListResult?.ErrorMessage ?? "Failed to retrieve expenses." });
    }

    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetExpense(string? id)
    {
        GetExpenseQuery query = new GetExpenseQuery(id.IsGuid() ? id!.ToGuid() : null);

        var expenseResult = await _sender.Send(query);
        if (expenseResult is not null && expenseResult.IsSuccess)
        {
            return Ok(expenseResult);
        }
        return BadRequest(new { errorMessage = expenseResult?.ErrorMessage ?? "Failed to retrieve expense." });
    }

    [HttpDelete()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteExpense(string? id)
    {
        if (string.IsNullOrEmpty(id) || !id.IsGuid())
        {
            _logger.LogInformation("Delete Expense Id is not valid {id}.", id);
            return BadRequest(new { errorMessage = "Not a valid Id." });
        }
        DeleteExpenseCommand command = new DeleteExpenseCommand()
        {
            Id = id!.ToGuid().Value
        };

        var expenseDeleteResult = await _sender.Send(command);
        if (expenseDeleteResult is not null && expenseDeleteResult.IsSuccess)
        {
            return Ok();
        }
        return StatusCode((int)HttpStatusCode.InternalServerError, new { errorMessage = expenseDeleteResult?.ErrorMessage ?? "Failed to delete expense" });
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
            _logger.LogInformation("Update expense request is null.");
            return BadRequest(new { errorMessage = "Request cannot be null" });
        }
        if (!ModelState.IsValid)
        {
            _logger.LogInformation("Model state is invalid: {ModelState}", ModelState);
            return BadRequest(ModelState);
        }
        var result = await _sender.Send(request);

        if (result is not null && result.IsSuccess)
        {
            return Ok(new { message = "Expense updated successfully" });
        }
        return StatusCode((int)HttpStatusCode.InternalServerError, new { errorMessage = result?.ErrorMessage ?? "Failed to update expense" });
    }


}
