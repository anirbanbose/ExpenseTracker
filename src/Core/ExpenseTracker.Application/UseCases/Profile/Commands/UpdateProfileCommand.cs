using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Application.UseCases.Profile.Commands;

public class UpdateProfileCommand : IRequest<Result>
{
    [Required(ErrorMessage = "Please enter first name")]
    [MaxLength(100, ErrorMessage = "First name can't be more than 100 characters")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "Please enter last name")]
    [MaxLength(100, ErrorMessage = "Last name can't be more than 100 characters")]
    public string LastName { get; set; }
    [MaxLength(100, ErrorMessage = "Middle name can't be more than 100 characters")]
    public string? MiddleName { get; set; }
}
