using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Application.UseCases.User.Commands;

public class ChangePasswordCommand : IRequest<Result>
{
    [Required(ErrorMessage = "Please enter current password")]
    public string CurrentPassword { get; set; }
    [Required(ErrorMessage = "Please enter new password")]
    [MaxLength(18, ErrorMessage = "New Password can't be more than 18 characters")]
    [MinLength(6, ErrorMessage = "New Password can't be less than 6 characters")]
    public string NewPassword { get; set; }
    [Required(ErrorMessage = "Please enter confirm password")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}
