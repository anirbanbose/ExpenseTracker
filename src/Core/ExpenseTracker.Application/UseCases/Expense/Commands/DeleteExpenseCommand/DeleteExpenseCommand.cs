using ExpenseTracker.Application.Validation;
using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Application.UseCases.Expense.Commands;

public class DeleteExpenseCommand : IRequest<Result>
{
    [Required(ErrorMessage = "Id is required")]
    [NotEmptyGuid(ErrorMessage = "Id is required")]
    public Guid Id { get; set; }
}