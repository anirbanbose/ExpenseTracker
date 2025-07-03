using ExpenseTracker.Application.DTO.Currency;

namespace ExpenseTracker.Application.DTO.Report;

public record ExpenseReportCurrencyGroupDTO(CurrencyDTO? currency, string formattedTotalAmmount, List<ExpenseReportDataDTO> reportData);
