using ExpenseTracker.Application.DTO.Expense;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;

namespace ExpenseTracker.Application.UseCases.Expense.Queries;

public record GetExpenseQuery(Guid? Id) : IRequest<Result<ExpenseDTO>>;

