using ExpenseTracker.Application.Validation;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Application.UseCases.ExpenseCategory.Commands;

public class AddNewExpenseCategoryCommand : IRequest<Result<Guid?>>
{
    [Required(ErrorMessage = "Please enter expense category name")]
    [MaxLength(100, ErrorMessage = "Expense category name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;
}