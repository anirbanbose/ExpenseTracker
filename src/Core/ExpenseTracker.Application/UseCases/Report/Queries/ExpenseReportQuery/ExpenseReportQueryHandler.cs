using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.Contracts.Report;
using ExpenseTracker.Application.DTO.Report;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.Persistence.SearchModels;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Report.Queries;

public class ExpenseReportQueryHandler : IRequestHandler<ExpenseReportQuery, Result<byte[]>>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly IExpenseRepository _expenseRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ILogger<ExpenseReportQueryHandler> _logger;
    private readonly IExcelReportGenerator<ExpenseReportDTO> _excelReportGenerator;
    private readonly IPdfReportGenerator<ExpenseReportDTO> _pdfReportGenerator;

    public ExpenseReportQueryHandler(IAuthenticatedUserProvider authProvider, IExpenseRepository expenseRepository, ICurrencyRepository currencyRepository, IExpenseCategoryRepository expenseCategoryRepository, IExcelReportGenerator<ExpenseReportDTO> excelReportGenerator,  IPdfReportGenerator<ExpenseReportDTO> pdfReportGenerator, ILogger<ExpenseReportQueryHandler> logger) 
    {
        _authProvider = authProvider;
        _expenseRepository = expenseRepository;
        _currencyRepository = currencyRepository;
        _excelReportGenerator = excelReportGenerator;
        _pdfReportGenerator = pdfReportGenerator;
        _logger = logger;
    }

    public async Task<Result<byte[]>> Handle(ExpenseReportQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (currentUser, failureResult) = await _authProvider.GetAuthenticatedUserAsync<Result<byte[]>>(new ResultUserNotAuthenticatedFactory<byte[]>(), cancellationToken);
            if (failureResult != null)
                return failureResult;

            DateTime? startDate = null;
            DateTime? endDate = null;

            if(request.reportType == Domain.Enums.ExpenseReportType.Yearly)
            {
                startDate = new DateTime(request.year, 1, 1);
                endDate = new DateTime(request.year, 12, 31);
            }
            else
            {
                startDate = new DateTime(request.year, request.month.Value, 1);
                int daysInMonth = DateTime.DaysInMonth(request.year, request.month.Value);
                endDate = new DateTime(request.year, request.month.Value, daysInMonth);
            }

            if(currentUser?.Preference is null)
            {
                _logger?.LogWarning($"User preference not present for the user: {_authProvider.CurrentUserName}.");
                return Result<byte[]>.FailureResult("Report.ExpenseReport");
            }

            var expenseResult = await _expenseRepository.SearchExpensesAsync(ExpenseSearchModel.Create(null, null, startDate, endDate), currentUser.Id, Domain.Enums.ExpenseListOrder.ExpenseDate, true, cancellationToken);
            if (expenseResult is not null)
            {
                var currency = await _currencyRepository.GetCurrencyByIdAsync(currentUser!.Preference!.PreferredCurrencyId, cancellationToken);
                if (currency is null)
                {
                    return Result<byte[]>.FailureResult("Report.ExpenseReport");
                }

                var totalAmmount = expenseResult.Sum(d => d.ExpenseAmount.Amount);
                var itemList = expenseResult.Select(d => new ExpenseReportDataItemDTO(d.ExpenseDate, d.Category.Name, d.Description, d.ExpenseAmount.ToString())).ToList();

                ExpenseReportDTO expenseReport = new ExpenseReportDTO(request.reportType, request.year, request.month, new Money(totalAmmount, currency.Code, currency.Symbol).ToString(), itemList);
                var report = request.reportFormat == Domain.Enums.ReportFormat.Pdf ? _pdfReportGenerator.GenerateAsync(expenseReport) : _excelReportGenerator.GenerateAsync(expenseReport);
                 
                return Result<byte[]>.SuccessResult(report.Result);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while exporting expense report- {request} for the user: {_authProvider.CurrentUserName}.");
        }


        return Result<byte[]>.FailureResult("Report.ExpenseExport");
    }
}