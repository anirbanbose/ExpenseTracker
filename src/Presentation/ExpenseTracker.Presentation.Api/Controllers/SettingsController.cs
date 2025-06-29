using ExpenseTracker.Application.UseCases.UserPreference.Commands;
using ExpenseTracker.Application.UseCases.UserPreference.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExpenseTracker.Presentation.Api.Controllers;

[Route("api/settings")]
[ApiController]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(ILogger<SettingsController> logger, ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    [HttpGet("user-preference")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserPreference()
    {
        GetUserPreferenceQuery query = new GetUserPreferenceQuery();

        var userPreferenceResult = await _sender.Send(query);
        if (userPreferenceResult is not null && userPreferenceResult.IsSuccess)
        {
            return Ok(userPreferenceResult);
        }
        return StatusCode((int)HttpStatusCode.InternalServerError, new { errorMessage = userPreferenceResult?.ErrorMessage ?? "Failed to retrieve user settings." });
    }

    [HttpPost("save-user-preference")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveUserPreference([FromBody] SaveUserPreferenceCommand request)
    {
        if (request == null)
        {
            _logger.LogInformation("Save user preference request is null.");
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
            return Ok(new { message = "User settings saved successfully." });
        }
        return StatusCode((int)HttpStatusCode.InternalServerError, new { errorMessage = result?.ErrorMessage ?? "Failed to save user settings." });
    }
}
