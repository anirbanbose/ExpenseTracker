using ExpenseTracker.Application.UseCases.Expense.Commands;
using ExpenseTracker.Application.UseCases.Profile.Commands;
using ExpenseTracker.Application.UseCases.Profile.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserProfile()
        {
            GetUserProfileQuery getUserProfileQuery = new GetUserProfileQuery();
            var profileResult = await _sender.Send(getUserProfileQuery);
            if (profileResult.IsSuccess && profileResult.Value != null)
            {
                return Ok(profileResult);
            }
            return BadRequest(new { errorMessage = profileResult.ErrorMessage ?? "Failed to retrieve Profile record." });
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
                return BadRequest(new { errorMessage = "Request cannot be null" });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _sender.Send(request);

            if (result.IsSuccess)
            {
                return Ok(new { message = "Profile updated successfully" });
            }
            return BadRequest(new { errorMessage = result.ErrorMessage ?? "Failed to update profile." });
        }
    }
}
