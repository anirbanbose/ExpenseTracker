using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.Contracts.Report;
using ExpenseTracker.Application.DTO.Report;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Report.Queries;

public class ExpenseExportQueryHandler : IRequestHandler<ExpenseExportQuery, Result<byte[]>>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly IExpenseRepository _expenseRepository;
    private readonly ILogger<ExpenseExportQueryHandler> _logger;
    private readonly IExcelReportGenerator<List<ExpenseReportDataItemDTO>> _excelExportGenerator;
    private readonly IPdfReportGenerator<List<ExpenseReportDataItemDTO>> _pdfExportGenerator;

    public ExpenseExportQueryHandler(IAuthenticatedUserProvider authProvider, IExpenseRepository expenseRepository, IExcelReportGenerator<List<ExpenseReportDataItemDTO>> excelExportGenerator, IPdfReportGenerator<List<ExpenseReportDataItemDTO>> pdfExportGenerator, ILogger<ExpenseExportQueryHandler> logger) 
    {
        _authProvider = authProvider;
        _expenseRepository = expenseRepository;
        _excelExportGenerator = excelExportGenerator;
        _pdfExportGenerator = pdfExportGenerator;
        _logger = logger;
    }

    public async Task<Result<byte[]>> Handle(ExpenseExportQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (currentUser, failureResult) = await _authProvider.GetAuthenticatedUserAsync<Result<byte[]>>(new ResultUserNotAuthenticatedFactory<byte[]>(), cancellationToken);
            if (failureResult != null)
                return failureResult;

            var expenseResult = await _expenseRepository.SearchExpensesAsync(request.search, request.expenseCategoryId, request.startDate, request.endDate, currentUser!.Id, request.order, request.IsAscendingSort, cancellationToken);
            if (expenseResult is not null)
            {
                var reportData = new List<ExpenseReportDataItemDTO>();
                expenseResult.ToList().ForEach(category =>
                {
                    reportData.Add(ExpenseReportDataItemDTO.FromDomain(category));
                });
                var report = request.reportFormat == Domain.Enums.ReportFormat.Pdf ? _pdfExportGenerator.GenerateAsync(reportData) : _excelExportGenerator.GenerateAsync(reportData);

                return Result<byte[]>.SuccessResult(report.Result);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while exporting expense report- {request} for the user: {_authProvider.CurrentUserName}.");
        }
        return Result<byte[]>.FailureResult();
    }
}