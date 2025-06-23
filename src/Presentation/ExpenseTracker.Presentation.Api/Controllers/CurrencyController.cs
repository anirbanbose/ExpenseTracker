using ExpenseTracker.Application.UseCases.Currency.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Presentation.Api.Controllers
{
    [Route("api/currency")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ILogger<CurrencyController> _logger;

        public CurrencyController(ISender sender, ILogger<CurrencyController> logger)
        {
            _sender = sender;
            _logger = logger;
        }

        [HttpGet("currencies")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCurrencies()
        {
            GetAllCurrenciesQuery getAllCurrenciesQuery = new GetAllCurrenciesQuery();
            var currencyListResult = await _sender.Send(getAllCurrenciesQuery);
            if (currencyListResult.IsSuccess && currencyListResult.Value != null)
            {
                return Ok(currencyListResult.Value);
            }
            return BadRequest(new { errorMessage = currencyListResult.ErrorMessage ?? "Failed to retrieve currencies." });
        }

        [HttpGet("currencies-select")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCurrenciesForSelect()
        {
            GetCurrenciesForSelectQuery getAllCurrenciesSelectQuery = new GetCurrenciesForSelectQuery();
            var currencyListResult = await _sender.Send(getAllCurrenciesSelectQuery);
            if (currencyListResult.IsSuccess && currencyListResult.Value != null)
            {
                return Ok(currencyListResult.Value);
            }
            return BadRequest(new { errorMessage = currencyListResult.ErrorMessage ?? "Failed to retrieve currencies." });
        }

        [HttpGet("preferred-currency")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> GetPreferredCurrency()
        {
            GetCurrentUserPreferredCurrencyQuery getPreferredCurrencyQuery = new GetCurrentUserPreferredCurrencyQuery();
            var currencyResult = await _sender.Send(getPreferredCurrencyQuery);
            if (currencyResult.IsSuccess && currencyResult.Value != null)
            {
                return Ok(currencyResult.Value);
            }
            return BadRequest(new { errorMessage = currencyResult.ErrorMessage ?? "Failed to retrieve preferred currency." });
        }
    }
}
