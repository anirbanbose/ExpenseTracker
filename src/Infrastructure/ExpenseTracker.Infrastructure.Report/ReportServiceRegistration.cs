using ExpenseTracker.Application.Contracts.Report;
using ExpenseTracker.Application.DTO.Report;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Infrastructure.Report;

public static class ReportServiceRegistration
{
    public static IServiceCollection AddReportServices(this IServiceCollection services)
    {
        services.AddScoped<IExcelReportGenerator<ExpenseExportDTO>, ExpenseExportExcelGenerator>();



        services.AddScoped<IPdfReportGenerator<ExpenseExportDTO>, ExpenseExportPdfGenerator>();

        return services;
    }
}
