using ExpenseTracker.Application.DTO.Dashboard;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;

namespace ExpenseTracker.Application.UseCases.Dashboard.RecentExpensesQuery;

public record RecentExpensesQuery(int recordCount) : IRequest<Result<List<RecentExpenseListDTO>>>;
