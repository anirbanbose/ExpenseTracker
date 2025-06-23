using ExpenseTracker.Application.UseCases.User.Commands;
using ExpenseTracker.Application.UseCases.User.Queries;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Presentation.Api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ISender _sender; 
        private readonly ILogger<AccountController> _logger;
        public AccountController(ILogger<AccountController> logger, ISender sender)
        {
            _logger = logger;
            _sender = sender;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginQuery request)
        {
            if(request == null)
            {
                return BadRequest(new { errorMessage = "Request cannot be null" });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var loginResponse = await _sender.Send(request);
            if (loginResponse != null && loginResponse.IsSuccess)
            {
                return Ok(new
                {
                    User = loginResponse.Value,
                    IsLoggedIn = true
                });
            }

            return Unauthorized(new
            {
                ErrorMessage = loginResponse?.ErrorMessage ?? "Invalid login attempt",
                IsLoggedIn = false
            });
        }

        [HttpGet("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Logout()
        {
            // Clear the authentication cookie
            Response.Cookies.Delete(Constants.ACCESS_TOKEN_NAME);
            return Ok(new { message = "Logged out" });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var username = User.Identity?.Name;

            return Ok(new
            {
                Username = username
            });
        }

        [Authorize]
        [HttpGet("loggedin-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetLoggedinUser()
        {
            LoggedinUserQuery query = new LoggedinUserQuery();
            var userResponse = await _sender.Send(query);
            if (userResponse != null && userResponse.IsSuccess)
            {
                return Ok(userResponse.Value);
            }
            return Unauthorized(new
            {
                ErrorMessage = userResponse?.ErrorMessage ?? "Invalid user",
                IsLoggedIn = false
            });
        }

        [Authorize]
        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand request)
        {
            if (request == null)
            {
                return BadRequest(new { errorMessage = "Request cannot be null" });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var changePasswordResponse = await _sender.Send(request);
            if (changePasswordResponse != null)
            {
                if(changePasswordResponse.IsSuccess)
                {
                    return Ok(new { message = "Password changed successfully" });
                }
                else if(changePasswordResponse.Code == Constants.AuthenticationErrorCode)
                {
                    return Unauthorized(new {
                        errorMessage = changePasswordResponse?.ErrorMessage ?? "User unauthorized." });
                }
                return BadRequest(new { errorMessage = changePasswordResponse?.ErrorMessage ?? "Failed to Change password" });

            }
            return BadRequest(new { errorMessage = "Failed to Change password" });
        }
    }
}
