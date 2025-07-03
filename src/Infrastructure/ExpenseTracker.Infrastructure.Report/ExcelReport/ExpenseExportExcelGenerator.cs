using ClosedXML.Excel;
using ExpenseTracker.Application.Contracts.Report;
using ExpenseTracker.Application.DTO.Report;

namespace ExpenseTracker.Infrastructure.Report;

public class ExpenseExportExcelGenerator : IExcelReportGenerator<List<ExpenseReportDataDTO>>
{
    public Task<byte[]> GenerateAsync(List<ExpenseReportDataDTO> exportData)
    {
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Expense Export");
        SetColumnWidth(ws, 1, 20);
        SetColumnWidth(ws, 2, 30);
        SetColumnWidth(ws, 3, 50);
        SetColumnWidth(ws, 4, 25);

        int rowNumber = 1;
        int tableStartRowNumber = rowNumber;

        rowNumber = AddTableRow(ws, rowNumber, "Date", "Category", "Description", "Amount", true);

        foreach (var expense in exportData)
        {
            rowNumber = AddTableRow(ws, rowNumber, expense.ExpenseDate.Date.ToString("dd MMM yyyy"), expense.Category, expense.Description, expense.ExpenseAmount);
        }
        var table = ws.Range($"A{tableStartRowNumber}:D{rowNumber - 1}").CreateTable();
        table.Theme = XLTableTheme.TableStyleLight16;

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
