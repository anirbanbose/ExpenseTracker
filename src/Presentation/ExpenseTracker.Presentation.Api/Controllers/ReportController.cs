using ExpenseTracker.Application.UseCases.Report.Queries;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExpenseTracker.Presentation.Api.Controllers;

[Route("api/report")]
[ApiController]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly ISender _sender;

    public ReportController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("expense-export")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExpenseExport(string? search, string? categoryId, string? currencyId, string? startDate, string? endDate, ExpenseListOrder order = ExpenseListOrder.ExpenseDate, bool isAscending = false, ReportFormat reportFormat = ReportFormat.Excel )
    {
        ExpenseExportQuery query = new ExpenseExportQuery(
            search: search,
            expenseCategoryId: categoryId.IsGuid() ? categoryId!.ToGuid() : null,
            currencyId: currencyId.IsGuid() ? currencyId!.ToGuid() : null,
            startDate: startDate.IsDate() ? startDate!.ToDate() : null,
            endDate: endDate.IsDate() ? endDate!.ToDate() : null,
            order: order,
            IsAscendingSort: isAscending,
            reportFormat: reportFormat
        );

        var expenseExportResult = await _sender.Send(query);
        if (expenseExportResult is not null && expenseExportResult.IsSuccess)
        {
            string extension = reportFormat == ReportFormat.Pdf ? "pdf" : "xlsx";
            string contentType = reportFormat == ReportFormat.Pdf
                ? "application/pdf"
                : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(expenseExportResult.Value, contentType, $"Expense Export.{extension}");
        }
        return StatusCode((int)HttpStatusCode.InternalServerError, new { errorMessage = expenseExportResult?.ErrorMessage ?? "Failed to generate export file." });
    }

    [HttpGet("min-max-dates")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMinMaxDates()
    {
        MinMaxExpenseDateQuery query = new MinMaxExpenseDateQuery();

        var minMaxDateResult = await _sender.Send(query);
        if (minMaxDateResult is not null && minMaxDateResult.IsSuccess)
        {
            return Ok(minMaxDateResult.Value);
        }
        return StatusCode((int)HttpStatusCode.InternalServerError, new { errorMessage = minMaxDateResult?.ErrorMessage ?? "Failed to retrieve min & max dates." });
    }

    [HttpGet("expense-report")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExpenseReport(ExpenseReportType reportType, int year, int? month, ReportFormat reportFormat = ReportFormat.Excel)
    {
        ExpenseReportQuery query = new ExpenseReportQuery(
            reportType,
            year,
            month,
            reportFormat
        );

        var expenseReportResult = await _sender.Send(query);
        if (expenseReportResult is not null && expenseReportResult.IsSuccess)
        {
            string extension = reportFormat == ReportFormat.Pdf ? "pdf" : "xlsx";
            string contentType = reportFormat == ReportFormat.Pdf
                ? "application/pdf"
                : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(expenseReportResult.Value, contentType, $"Expense Export.{extension}");
        }
        return StatusCode((int)HttpStatusCode.InternalServerError, new { errorMessage = expenseReportResult?.ErrorMessage ?? "Failed to generate report file." });
    }
}
