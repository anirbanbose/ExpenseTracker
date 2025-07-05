using ExpenseTracker.Application.DTO.Expense;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;

namespace ExpenseTracker.Application.UseCases.Expense.Queries;

public record SearchExpensesQuery(string? search, Guid? expenseCategoryId, DateTime? startDate, DateTime? endDate, int pageIndex, int pageSize, ExpenseListOrder order = ExpenseListOrder.ExpenseDate, bool IsAscendingSort = false) : IRequest<PagedResult<ExpenseListDTO>>;

