using ExpenseTracker.Application.Contracts.Report;
using ExpenseTracker.Application.DTO.Report;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace ExpenseTracker.Infrastructure.Report;

public class ExpenseExportPdfGenerator : IPdfReportGenerator<ExpenseExportDTO>
{
    public Task<byte[]> GenerateAsync(ExpenseExportDTO exportData)
    {
        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);

                page.Header().Height(30).Background(Colors.Grey.Lighten1).AlignCenter().AlignMiddle().Text("Expense Export").FontSize(18).Bold();

                page.Content().Background(Colors.White).Padding(5).Table(table =>
                {
                    table.ColumnsDefinition(columns => { columns.RelativeColumn(); columns.RelativeColumn(); ; columns.RelativeColumn(); ; columns.RelativeColumn(); });

                    table.Header(header =>
                    {
                        header.Cell().Text("Date").Bold();
                        header.Cell().Text("Category").Bold();
                        header.Cell().Text("Description").Bold();
                        header.Cell().AlignRight().Text("Amount").Bold();
                    });

                    foreach (var expense in exportData.ReportData)
                    {
                        table.Cell().Text(expense.ExpenseDate.Date.ToString("dd MMM yyyy"));
                        table.Cell().Text(expense.Category);
                        table.Cell().Text(expense.Description);
                        table.Cell().AlignRight().Text(expense.ExpenseAmount);
                    }
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