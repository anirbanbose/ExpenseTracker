using ExpenseTracker.Application.Validation;
using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Application.UseCases.ExpenseCategory.Commands;

public class UpdateExpenseCategoryCommand : IRequest<Result>
{
    [Required(ErrorMessage = "Id is required")]
    [NotEmptyGuid(ErrorMessage = "Id is required")]
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Please enter expense category name")]
    [MaxLength(100, ErrorMessage = "Expense category name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;
}