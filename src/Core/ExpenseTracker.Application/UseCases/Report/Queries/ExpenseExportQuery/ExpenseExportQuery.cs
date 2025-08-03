using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;

namespace ExpenseTracker.Application.UseCases.Report.Queries;

public record ExpenseExportQuery(string? search, Guid? expenseCategoryId, DateTime? startDate, DateTime? endDate, ExpenseListOrder order = ExpenseListOrder.ExpenseDate, bool IsAscendingSort = false, ReportFormat reportFormat = ReportFormat.Excel) : IRequest<Result<byte[]>>;