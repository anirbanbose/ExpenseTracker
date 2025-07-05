using ExpenseTracker.Application.Contracts.Report;
using ExpenseTracker.Application.DTO.Report;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.Persistence.SearchModels;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Report.Queries;

public class ExpenseExportQueryHandler : BaseHandler, IRequestHandler<ExpenseExportQuery, Result<byte[]>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ExpenseExportQueryHandler> _logger;
    private readonly IExcelReportGenerator<List<ExpenseReportDataItemDTO>> _excelExportGenerator;
    private readonly IPdfReportGenerator<List<ExpenseReportDataItemDTO>> _pdfExportGenerator;

    public ExpenseExportQueryHandler(IHttpContextAccessor httpContextAccessor, IExpenseRepository expenseRepository, IUserRepository userRepository, IExpenseCategoryRepository expenseCategoryRepository, IExcelReportGenerator<List<ExpenseReportDataItemDTO>> excelExportGenerator, IPdfReportGenerator<List<ExpenseReportDataItemDTO>> pdfExportGenerator, ILogger<ExpenseExportQueryHandler> logger) : base(httpContextAccessor)
    {
        _expenseRepository = expenseRepository;
        _userRepository = userRepository;
        _expenseCategoryRepository = expenseCategoryRepository;
        _excelExportGenerator = excelExportGenerator;
        _pdfExportGenerator = pdfExportGenerator;
        _logger = logger;
    }

    public async Task<Result<byte[]>> Handle(ExpenseExportQuery request, CancellationToken cancellationToken)
    {
        try
        {            
            var (currentUser, failureResult) = await GetAuthenticatedUserAsync(_userRepository, _logger, cancellationToken, new ResultUserNotAuthenticatedFactory<byte[]>());
            if (failureResult != null)
                return failureResult;

            var expenseResult = await _expenseRepository.SearchExpensesAsync(ExpenseSearchModel.Create(request.search, request.expenseCategoryId, request.startDate, request.endDate), currentUser.Id, request.order, request.IsAscendingSort, cancellationToken);
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
            _logger?.LogError(ex, $"Error occurred while exporting expense report- {request} for the user: {CurrentUserName}.");
        }
        

        return Result<byte[]>.FailureResult("Report.ExpenseExport", "Couldn't generate expense export file.");
    }
}