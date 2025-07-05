using ExpenseTracker.Application.Contracts.Report;
using ExpenseTracker.Application.DTO.Report;
using ExpenseTracker.Domain.Utils;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace ExpenseTracker.Infrastructure.Report;

public class ExpenseExportPdfGenerator : IPdfReportGenerator<List<ExpenseReportDataItemDTO>>
{
    public Task<byte[]> GenerateAsync(List<ExpenseReportDataItemDTO> exportData)
    {
        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);

                page.Header().Height(30).Background(Colors.Grey.Lighten1).AlignCenter().AlignMiddle().Text("Expense Export").FontSize(18).Bold();
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
                        foreach (var expense in exportData)
                        {
                            rowCount++;
                            table.Cell().Text(rowCount.ToString());
                            table.Cell().Text(expense.ExpenseDate.Date.ToString("dd MMM yyyy"));
                            table.Cell().Text(expense.Category);
                            table.Cell().Text(expense.Description?.Truncate(50, true));
                            table.Cell().AlignRight().Text(expense.ExpenseAmount);
                        }
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