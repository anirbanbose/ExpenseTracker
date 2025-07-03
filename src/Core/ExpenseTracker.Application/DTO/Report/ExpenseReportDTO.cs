
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.DTO.Report;

public record ExpenseReportDTO(ExpenseReportType reportType, int year, int? month, List<ExpenseReportCurrencyGroupDTO> groupedExpenseReportData);
