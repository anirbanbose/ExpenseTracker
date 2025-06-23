using ExpenseTracker.Application.DTO.User;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Application.UseCases.User.Queries;

public class LoginQuery : IRequest<Result<LoggedInUserDTO>>
{
    [Required(ErrorMessage = "Please enter your email")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Please enter your password")]
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}

