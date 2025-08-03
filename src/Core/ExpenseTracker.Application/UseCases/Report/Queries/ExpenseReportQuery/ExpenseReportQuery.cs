using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;

namespace ExpenseTracker.Application.UseCases.Report.Queries;

public record ExpenseReportQuery(ExpenseReportType reportType, int year, int? month, ReportFormat reportFormat = ReportFormat.Excel) : IRequest<Result<byte[]>>;