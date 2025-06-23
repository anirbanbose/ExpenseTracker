using ExpenseTracker.Application.DTO.ExpenseCategory;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;

namespace ExpenseTracker.Application.UseCases.ExpenseCategory.Queries;  

public record GetAllExpenseCategoriesByUserIdQuery() : IRequest<Result<List<ExpenseCategoryDTO>>>;

