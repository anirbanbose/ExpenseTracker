using ExpenseTracker.Application.UseCases.User.Commands;
using ExpenseTracker.Application.UseCases.User.Queries;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        public async Task<IActionResult> Login([FromBody] LoginQuery request)
        {
            if(request == null)
            {
                _logger.LogInformation("Login request is null.");
                return BadRequest(new { errorMessage = "Request cannot be null" });
            }
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Model state is invalid: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }
            var loginResponse = await _sender.Send(request);
            if (loginResponse is not null && loginResponse.IsSuccess)
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
            Response.Cookies.Delete(Constants.ACCESS_TOKEN_NAME);
            return Ok(new { message = "Logged out" });
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegistrationCommand request)
        {
            if (request == null)
            {
                return BadRequest(new { errorMessage = "Request cannot be null" });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var registrationResponse = await _sender.Send(request);
            if (registrationResponse is not null && registrationResponse.IsSuccess)
            {
                return Ok(new { message = "Regitration Success" });
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, new { errorMessage = registrationResponse?.ErrorMessage ?? "Registration failed. Please try again later." });            
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                        errorMessage = changePasswordResponse.ErrorMessage ?? "User unauthorized." });
                }
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, new { errorMessage = changePasswordResponse?.ErrorMessage ?? "Failed to Change password" });
        }


        [HttpGet("email-available")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> EmailAvailable(string email)
        {
            EmailAvailableQuery query = new EmailAvailableQuery(email);
            return Ok(await _sender.Send(query));
        }
    }
}
