
namespace ExpenseTracker.Application.DTO.Dashboard;

public class SpendingChartDTO
{
    public List<string> Labels { get; set; }
    public List<SpendingChartDataDTO> Datasets { get; set; }
}

public class SpendingChartDataDTO
{
    public string Label { get; set; }
    public List<decimal> Data { get; set; }
}
