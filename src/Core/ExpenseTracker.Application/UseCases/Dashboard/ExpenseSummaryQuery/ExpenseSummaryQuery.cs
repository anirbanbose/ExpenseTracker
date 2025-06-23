using ExpenseTracker.Application.DTO.Dashboard;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;

namespace ExpenseTracker.Application.UseCases.Dashboard;

public record ExpenseSummaryQuery(int YearTotalExpenseRecordCount, int MonthTotalExpenseRecordCount, int CategoryTotalExpenseRecordCount) : IRequest<Result<ExpenseSummaryDTO>>;