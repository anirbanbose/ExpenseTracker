using ExpenseTracker.Application.UseCases.Report;
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
}
