using ExpenseTracker.Application.Validation;
using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Application.UseCases.Expense.Commands;

public class UpdateExpenseCommand : IRequest<Result>
{
    [Required(ErrorMessage = "Id is required")]
    [NotEmptyGuid(ErrorMessage = "Id is required")]
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Please enter expense amount")]
    public decimal Amount { get; set; } = 0.0m;
    public string? Description { get; set; }
    [Required(ErrorMessage = "Please enter expense date")]
    public string ExpenseDate { get; set; } = DateTime.UtcNow.Date.ToString();
    [Required(ErrorMessage = "Please select expense category")]
    [NotEmptyGuid(ErrorMessage = "Please select expense category")]
    public Guid CategoryId { get; set; }
}
