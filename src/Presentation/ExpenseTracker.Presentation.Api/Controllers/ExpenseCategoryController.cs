using ExpenseTracker.Application.UseCases.ExpenseCategory.Commands;
using ExpenseTracker.Application.UseCases.ExpenseCategory.Queries;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExpenseTracker.Presentation.Api.Controllers
{
    [Authorize]
    [Route("api/expense-category")]
    [ApiController]
    public class ExpenseCategoryController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ILogger<ExpenseCategoryController> _logger;

        public ExpenseCategoryController(ISender sender, ILogger<ExpenseCategoryController> logger)
        {
            _sender = sender;
            _logger = logger;
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetExpenseCategoryById(string? Id)
        {
            GetExpenseCategoryByIdQuery getAllExpenseCategoriesByUserIdQuery = new GetExpenseCategoryByIdQuery(Id.IsGuid() ? Id!.ToGuid() : null);
            var expenseCategoryListResult = await _sender.Send(getAllExpenseCategoriesByUserIdQuery);
            if (expenseCategoryListResult is not null && expenseCategoryListResult.IsSuccess && expenseCategoryListResult.Value != null)
            {
                return Ok(expenseCategoryListResult.Value);
            }
            return BadRequest(new { errorMessage = expenseCategoryListResult?.ErrorMessage ?? "Failed to retrieve expense categories." });
        }


        [HttpGet("all-expense-categories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllExpenseCategoriesByUserId()
        {
            GetAllExpenseCategoriesByUserIdQuery getAllExpenseCategoriesByUserIdQuery = new GetAllExpenseCategoriesByUserIdQuery();
            var expenseCategoryListResult = await _sender.Send(getAllExpenseCategoriesByUserIdQuery);
            if (expenseCategoryListResult is not null && expenseCategoryListResult.IsSuccess && expenseCategoryListResult.Value != null)
            {
                return Ok(expenseCategoryListResult.Value);
            }
            return BadRequest(new { errorMessage = expenseCategoryListResult?.ErrorMessage ?? "Failed to retrieve expense categories." });
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SearchExpenseCategories(string? search, int pageIndex = 0, int pageSize = 10, ExpenseCategoryListOrder order = ExpenseCategoryListOrder.Name, bool isAscending = false)
        {
            SearchExpenseCategoriesQuery searchExpenseCategoriesQuery = new SearchExpenseCategoriesQuery(search, pageIndex, pageSize, order, isAscending);
            var expenseCategoryListResult = await _sender.Send(searchExpenseCategoriesQuery);
            if (expenseCategoryListResult is not null && expenseCategoryListResult.IsSuccess)
            {
                return Ok(expenseCategoryListResult);
            }
            return BadRequest(new { errorMessage = expenseCategoryListResult?.ErrorMessage ?? "Failed to retrieve expense categories." });
        }


        [HttpPost("add-new")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddNewExpenseCategory([FromBody] AddNewExpenseCategoryCommand request)
        {
            if (request == null)
            {
                _logger.LogInformation("Add expense category request is null.");
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
                return Ok(new { id = result.Value, message = "Expense category added successfully" });
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, new { errorMessage = result?.ErrorMessage ?? "Failed to add expense category" });
        }

        [HttpPost("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateExpenseCategory([FromBody] UpdateExpenseCategoryCommand request)
        {
            if (request == null)
            {
                _logger.LogInformation("Update expense category request is null.");
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
                return Ok(new { message = "Expense category updated successfully" });
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, new { errorMessage = result?.ErrorMessage ?? "Failed to update expense category" });
        }

        [HttpDelete()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteExpenseCategory(string? id)
        {
            if (string.IsNullOrEmpty(id) || !id.IsGuid())
            {

                _logger.LogInformation("Delete Expense category Id is not valid {id}.", id);
                return BadRequest(new { errorMessage = "Not a valid Id." });
            }
            DeleteExpenseCategoryCommand command = new DeleteExpenseCategoryCommand()
            {
                Id = id!.ToGuid().Value
            };

            var expenseCategoryDeleteResult = await _sender.Send(command);
            if (expenseCategoryDeleteResult is not null && expenseCategoryDeleteResult.IsSuccess)
            {
                return Ok();
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, new { errorMessage = expenseCategoryDeleteResult?.ErrorMessage ?? "Failed to delete expense category." });            
        }
    }
}
