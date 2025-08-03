using ExpenseTracker.Application.DTO.Report;
using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;

namespace ExpenseTracker.Application.UseCases.Report.Queries;
public record MinMaxExpenseDateQuery() : IRequest<Result<MinMaxExprenseDateDTO>>;