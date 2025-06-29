using ExpenseTracker.Application.DTO.ExpenseCategory;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;

namespace ExpenseTracker.Application.UseCases.ExpenseCategory.Queries;

public record SearchExpenseCategoriesQuery(string? Search, int PageIndex, int PageSize, ExpenseCategoryListOrder Order = ExpenseCategoryListOrder.Name, bool IsAscendingSort = true) : IRequest<PagedResult<ExpenseCategoryDTO>>;