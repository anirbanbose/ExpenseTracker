using ExpenseTracker.Application.DTO.ExpenseCategory;
using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;

namespace ExpenseTracker.Application.UseCases.ExpenseCategory.Queries;  

public record GetAllExpenseCategoriesByUserIdQuery() : IRequest<Result<List<ExpenseCategoryDTO>>>;

