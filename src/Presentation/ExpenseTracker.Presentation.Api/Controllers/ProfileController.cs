using ExpenseTracker.Application.UseCases.Profile.Commands;
using ExpenseTracker.Application.UseCases.Profile.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExpenseTracker.Presentation.Api.Controllers
{
    [Route("api/profile")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(ISender sender, ILogger<ProfileController> logger)
        {
            _sender = sender;
            _logger = logger;            
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserProfile()
        {
            GetUserProfileQuery getUserProfileQuery = new GetUserProfileQuery();
            var profileResult = await _sender.Send(getUserProfileQuery);
            if (profileResult is not null && profileResult.IsSuccess && profileResult.Value != null)
            {
                return Ok(profileResult);
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, new { errorMessage = profileResult?.ErrorMessage ?? "Failed to retrieve Profile record." });
        }

        [HttpPost("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand request)
        {
            if (request == null)
            {
                _logger.LogInformation("Update profile request is null.");
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
                return Ok(new { message = "Profile updated successfully" });
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, new { errorMessage = result?.ErrorMessage ?? "Failed to update profile." });
        }
    }
}
