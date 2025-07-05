using ExpenseTracker.Application.DTO.Dashboard;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;

namespace ExpenseTracker.Application.UseCases.Dashboard.Queries;

public record ExpenseSummaryQuery() : IRequest<Result<ExpenseSummaryDTO>>;