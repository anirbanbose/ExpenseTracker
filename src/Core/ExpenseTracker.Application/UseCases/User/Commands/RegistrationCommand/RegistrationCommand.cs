using ExpenseTracker.Application.Validation;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Application.UseCases.User.Commands;

public class RegistrationCommand : IRequest<Result>
{
    [Required(ErrorMessage = "Please enter your email")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address")]
    [MaxLength(250, ErrorMessage = "Email can't be more than 250 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your password")]
    [MaxLength(16, ErrorMessage = "Password can't be more than 16 characters.")]
    [MinLength(6, ErrorMessage = "Password can't be less than 6 characters.")]
    public string Password { get; set; } = string.Empty;    

    [Required(ErrorMessage = "Please enter your first name")]
    [MaxLength(100, ErrorMessage = "First name can't be more than 100 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your last name")]
    [MaxLength(100, ErrorMessage = "Last name can't be more than 100 characters.")]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "Middle name can't be more than 100 characters.")]
    public string? MiddleName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select your preferred currency")]
    [NotEmptyGuid(ErrorMessage = "Please select your preferred currency")]
    public Guid PreferredCurrencyId { get; set; }
}
