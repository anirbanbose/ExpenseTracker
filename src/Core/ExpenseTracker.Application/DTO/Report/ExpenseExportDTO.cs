namespace ExpenseTracker.Application.DTO.Report;

public class ExpenseExportDTO
{
    public string? SearchText { get; set; } = null;
    public string? Category { get; set; } = null;
    public DateTime? StartDate { get; set; } = null;
    public DateTime? EndDate { get; set;} = null;

    public List<ExpenseReportDTO> ReportData { get; set; } = new List<ExpenseReportDTO>();
}
