using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;

namespace ExpenseTracker.Application.UseCases.Report;

public record ExpenseExportQuery(string? search, Guid? expenseCategoryId, Guid? currencyId, DateTime? startDate, DateTime? endDate, ExpenseListOrder order = ExpenseListOrder.ExpenseDate, bool IsAscendingSort = false, ReportFormat reportFormat = ReportFormat.Excel) : IRequest<Result<byte[]>>;