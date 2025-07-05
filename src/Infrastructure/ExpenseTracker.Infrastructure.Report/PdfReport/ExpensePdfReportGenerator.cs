using ExpenseTracker.Application.Contracts.Report;
using ExpenseTracker.Application.DTO.Report;
using ExpenseTracker.Domain.Utils;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace ExpenseTracker.Infrastructure.Report.PdfReport;

public class ExpensePdfReportGenerator : IPdfReportGenerator<ExpenseReportDTO>
{
    public Task<byte[]> GenerateAsync(ExpenseReportDTO expenseReport)
    {
        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);

                string headerText = $"{(expenseReport.reportType == Domain.Enums.ExpenseReportType.Yearly ? "Yearly" : "Monthly")} Expense Report - {(expenseReport.reportType == Domain.Enums.ExpenseReportType.Monthly ? $"{UtilityServices.GetMonthName(expenseReport.month)} " : string.Empty)}{expenseReport.year}";
                page.Header().Height(30).Background(Colors.Grey.Lighten1).AlignCenter().AlignMiddle().Text(headerText).FontSize(18).Bold();
                
                page.Content().Background(Colors.White).Padding(5)
                .Column(column =>
                {
                    column.Item().DefaultTextStyle(t => t.FontSize(10)).Table(table =>
                    {
                        table.ColumnsDefinition(columns => { columns.ConstantColumn(30); columns.RelativeColumn(1); columns.RelativeColumn(2); ; columns.RelativeColumn(3); ; columns.RelativeColumn(1); });
                        
                        table.Header(header =>
                        {
                            header.Cell().Text("#").Bold();
                            header.Cell().Text("Date").Bold();
                            header.Cell().Text("Category").Bold();
                            header.Cell().Text("Description").Bold();
                            header.Cell().AlignRight().Text("Amount").Bold();
                        });
                        int rowCount = 0;
                        int totalCount = expenseReport.reportData.Count;
                        foreach (var expense in expenseReport.reportData)
                        {
                            rowCount++;
                            if (rowCount == totalCount)
                            {
                                table.Cell().PaddingBottom(8).Text(rowCount.ToString());
                                table.Cell().PaddingBottom(8).Text(expense.ExpenseDate.Date.ToString("dd MMM yyyy"));
                                table.Cell().PaddingBottom(8).Text(expense.Category);
                                table.Cell().PaddingBottom(8).Text(expense.Description?.Truncate(50, true));
                                table.Cell().PaddingBottom(8).AlignRight().Text(expense.ExpenseAmount);
                            }
                            else
                            {
                                table.Cell().Text(rowCount.ToString());
                                table.Cell().Text(expense.ExpenseDate.Date.ToString("dd MMM yyyy"));
                                table.Cell().Text(expense.Category);
                                table.Cell().Text(expense.Description?.Truncate(50, true));
                                table.Cell().AlignRight().Text(expense.ExpenseAmount);
                            }

                        }

                        table.Cell().ColumnSpan(4).BorderTop(1).Text("Total").Bold();
                        table.Cell().BorderTop(1).AlignRight().Text(expenseReport.formattedTotalAmmount).Bold();

                    });
                });                    

                page.Footer().Height(20).Background(Colors.Grey.Lighten1).AlignCenter().AlignMiddle().Text(x =>
                {
                    x.Span("Page ").FontSize(7);
                    x.CurrentPageNumber().FontSize(7);
                    x.Span(" of ").FontSize(7);
                    x.TotalPages().FontSize(7);
                });
            });
        });


        var stream = new MemoryStream();
        doc.GeneratePdf(stream);
        return Task.FromResult(stream.ToArray());
    }
}