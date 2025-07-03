using ExpenseTracker.Application.Contracts.Report;
using ExpenseTracker.Application.DTO.Report;
using ExpenseTracker.Infrastructure.Report.ExcelReport;
using ExpenseTracker.Infrastructure.Report.PdfReport;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Infrastructure.Report;

public static class ReportServiceRegistration
{
    public static IServiceCollection AddReportServices(this IServiceCollection services)
    {
        services.AddScoped<IExcelReportGenerator<List<ExpenseReportDataDTO>>, ExpenseExportExcelGenerator>();
        services.AddScoped<IPdfReportGenerator<List<ExpenseReportDataDTO>>, ExpenseExportPdfGenerator>();
        
        services.AddScoped<IExcelReportGenerator<ExpenseReportDTO>, ExpenseExcelReportGenerator>();
        services.AddScoped<IPdfReportGenerator<ExpenseReportDTO>, ExpensePdfReportGenerator>();

        return services;
    }
}
