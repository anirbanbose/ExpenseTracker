using ExpenseTracker.Application.UseCases.Currency.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExpenseTracker.Presentation.Api.Controllers
{
    [Route("api/currency")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ISender _sender;

        public CurrencyController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("currencies")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrencies()
        {
            GetAllCurrenciesQuery getAllCurrenciesQuery = new GetAllCurrenciesQuery();
            var currencyListResult = await _sender.Send(getAllCurrenciesQuery);
            if (currencyListResult is not null && currencyListResult.IsSuccess && currencyListResult.Value != null)
            {
                return Ok(currencyListResult.Value);
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, new { errorMessage = currencyListResult?.ErrorMessage ?? "Failed to retrieve currencies." });
        }

        [HttpGet("currencies-select")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrenciesForSelect()
        {
            GetCurrenciesForSelectQuery getAllCurrenciesSelectQuery = new GetCurrenciesForSelectQuery();
            var currencyListResult = await _sender.Send(getAllCurrenciesSelectQuery);
            if (currencyListResult is not null && currencyListResult.IsSuccess && currencyListResult.Value != null)
            {
                return Ok(currencyListResult.Value);
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, new { errorMessage = currencyListResult?.ErrorMessage ?? "Failed to retrieve currencies." });
        }

        [HttpGet("preferred-currency")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> GetPreferredCurrency()
        {
            GetCurrentUserPreferredCurrencyQuery getPreferredCurrencyQuery = new GetCurrentUserPreferredCurrencyQuery();
            var currencyResult = await _sender.Send(getPreferredCurrencyQuery);
            if (currencyResult is not null && currencyResult.IsSuccess && currencyResult.Value != null)
            {
                return Ok(currencyResult.Value);
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, new { errorMessage = currencyResult?.ErrorMessage ?? "Failed to retrieve preferred currency." });
        }
    }
}
