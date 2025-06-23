using ExpenseTracker.Application.UseCases.ExpenseCategory.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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


        [HttpGet("all-expense-categories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllExpenseCategoriesByUserId()
        {
            GetAllExpenseCategoriesByUserIdQuery getAllExpenseCategoriesByUserIdQuery = new GetAllExpenseCategoriesByUserIdQuery();
            var expenseCategoryListResult = await _sender.Send(getAllExpenseCategoriesByUserIdQuery);
            if (expenseCategoryListResult.IsSuccess && expenseCategoryListResult.Value != null)
            {
                return Ok(expenseCategoryListResult.Value);
            }
            return BadRequest(new { errorMessage = expenseCategoryListResult.ErrorMessage ?? "Failed to retrieve expense categories." });
        }
    }
}
