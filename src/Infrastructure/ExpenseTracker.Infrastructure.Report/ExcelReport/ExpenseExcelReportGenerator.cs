using ClosedXML.Excel;
using ExpenseTracker.Application.Contracts.Report;
using ExpenseTracker.Application.DTO.Report;
using ExpenseTracker.Domain.Utils;

namespace ExpenseTracker.Infrastructure.Report.ExcelReport;

public class ExpenseExcelReportGenerator : IExcelReportGenerator<ExpenseReportDTO>
{
    public Task<byte[]> GenerateAsync(ExpenseReportDTO expenseReport)
    {
        using var wb = new XLWorkbook();

        var ws = wb.AddWorksheet("Expense Report");
        SetColumnWidth(ws, 1, 20);
        SetColumnWidth(ws, 2, 30);
        SetColumnWidth(ws, 3, 50);
        SetColumnWidth(ws, 4, 25);

        ws.Cell("A1").Value = $"{(expenseReport.reportType == Domain.Enums.ExpenseReportType.Yearly ? "Yearly" : "Monthly")} Expense Report - {(expenseReport.reportType == Domain.Enums.ExpenseReportType.Monthly ? $"{UtilityServices.GetMonthName(expenseReport.month)} " : string.Empty)}{expenseReport.year}";
        ws.Cell("A1").Style.Font.FontSize = 18;
        ws.Cell("A1").Style.Font.Bold = true;
        ws.Cell("A1").Style.Fill.BackgroundColor = XLColor.LightGray;
        ws.Range("A1:D1").Merge();

        int rowNumber = 3;

        int tableStartRowNumber = rowNumber;

        rowNumber = AddTableRow(ws, rowNumber, "Date", "Category", "Description", "Amount", true);

        foreach (var expense in expenseReport.reportData)
        {
            rowNumber = AddTableRow(ws, rowNumber, expense.ExpenseDate.Date.ToString("dd MMM yyyy"), expense.Category, expense.Description, expense.ExpenseAmount);
        }
        var table = ws.Range($"A{tableStartRowNumber}:D{rowNumber - 1}").CreateTable();
        table.Theme = XLTableTheme.TableStyleLight16;
        ws.Cell($"A{rowNumber}").Value = "Total";
        ws.Cell($"A{rowNumber}").Style.Font.FontSize = 14;
        ws.Cell($"A{rowNumber}").Style.Font.Bold = true;
        ws.Cell($"A{rowNumber}").Style.Font.FontColor = XLColor.White;
        ws.Cell($"A{rowNumber}").Style.Fill.BackgroundColor = XLColor.Navy;
        ws.Range($"A{rowNumber}:C{rowNumber}").Merge();
        ws.Cell($"D{rowNumber}").Value = expenseReport.formattedTotalAmmount;
        ws.Cell($"D{rowNumber}").Style.Font.FontSize = 14;
        ws.Cell($"D{rowNumber}").Style.Font.Bold = true;
        ws.Cell($"D{rowNumber}").Style.Font.FontColor = XLColor.White;
        ws.Cell($"D{rowNumber}").Style.Fill.BackgroundColor = XLColor.Navy;
        ws.Cell($"D{rowNumber}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

        using var stream = new MemoryStream();
        wb.SaveAs(stream);
        return Task.FromResult(stream.ToArray());
    }

    private static int AddTableRow(IXLWorksheet ws, int rowNumber, string? Col1, string? Col2, string? Col3, string? Col4, bool setBold = false)
    {
        ws.Cell($"A{rowNumber}").SetValue(Col1);
        ws.Cell($"A{rowNumber}").Style.Font.FontSize = 14;
        ws.Cell($"A{rowNumber}").Style.Font.Bold = setBold;
        ws.Cell($"B{rowNumber}").SetValue(Col2);
        ws.Cell($"B{rowNumber}").Style.Font.FontSize = 14;
        ws.Cell($"B{rowNumber}").Style.Font.Bold = setBold;
        ws.Cell($"C{rowNumber}").SetValue(Col3);
        ws.Cell($"C{rowNumber}").Style.Font.FontSize = 14;
        ws.Cell($"C{rowNumber}").Style.Font.Bold = setBold;
        ws.Cell($"D{rowNumber}").SetValue(Col4);
        ws.Cell($"D{rowNumber}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
        ws.Cell($"D{rowNumber}").Style.Font.FontSize = 14;
        ws.Cell($"D{rowNumber}").Style.Font.Bold = setBold;
        rowNumber++;
        return rowNumber;
    }


    private static void SetColumnWidth(IXLWorksheet ws, int columnNumber, int width)
    {
        var col = ws.Column(columnNumber);
        col.Width = width;
    }
}