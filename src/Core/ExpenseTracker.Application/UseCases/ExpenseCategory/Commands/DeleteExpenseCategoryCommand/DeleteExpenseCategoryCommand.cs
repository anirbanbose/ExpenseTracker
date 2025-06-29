using ExpenseTracker.Application.Validation;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Application.UseCases.ExpenseCategory.Commands;

public class DeleteExpenseCategoryCommand : IRequest<Result>
{
    [Required(ErrorMessage = "Id is required")]
    [NotEmptyGuid(ErrorMessage = "Id is required")]
    public Guid Id { get; set; }
}