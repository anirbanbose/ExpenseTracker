using ExpenseTracker.Application.Contracts.Report;
using ExpenseTracker.Application.DTO.Currency;
using ExpenseTracker.Application.DTO.Report;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.Persistence.SearchModels;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Report.Queries;

public class ExpenseReportQueryHandler : BaseHandler, IRequestHandler<ExpenseReportQuery, Result<byte[]>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ExpenseReportQueryHandler> _logger;
    private readonly IExcelReportGenerator<ExpenseReportDTO> _excelReportGenerator;
    private readonly IPdfReportGenerator<ExpenseReportDTO> _pdfReportGenerator;

    public ExpenseReportQueryHandler(IHttpContextAccessor httpContextAccessor, IExpenseRepository expenseRepository, ICurrencyRepository currencyRepository, IUserRepository userRepository, IExpenseCategoryRepository expenseCategoryRepository, IExcelReportGenerator<ExpenseReportDTO> excelReportGenerator,  IPdfReportGenerator<ExpenseReportDTO> pdfReportGenerator, ILogger<ExpenseReportQueryHandler> logger) : base(httpContextAccessor)
    {
        _expenseRepository = expenseRepository;
        _userRepository = userRepository;
        _currencyRepository = currencyRepository;
        _excelReportGenerator = excelReportGenerator;
        _pdfReportGenerator = pdfReportGenerator;
        _logger = logger;
    }

    public async Task<Result<byte[]>> Handle(ExpenseReportQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (currentUser, failureResult) = await GetAuthenticatedUserAsync(_userRepository, _logger, cancellationToken, new ResultUserNotAuthenticatedFactory<byte[]>());
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

            var expenseResult = await _expenseRepository.SearchExpensesAsync(ExpenseSearchModel.Create(null, null, null, startDate, endDate), currentUser.Id, Domain.Enums.ExpenseListOrder.ExpenseDate, true, cancellationToken);
            if (expenseResult is not null)
            {
                var currencies = await _currencyRepository.GetAllCurrenciesAsync(cancellationToken);
                if (currencies is null || !currencies.Any())
                {
                    return Result<byte[]>.FailureResult("Report.ExpenseReport", "No Currencies found.");
                }

                var groupedByCurrency = expenseResult
                    .GroupBy(e => new { e.ExpenseAmount.CurrencyId, e.ExpenseAmount.CurrencyCode, e.ExpenseAmount.CurrencySymbol })
                    .Select(g => new ExpenseReportCurrencyGroupDTO(
                        currencies.Any(c => c.Id == g.Key.CurrencyId) ? CurrencyDTO.FromDomain(currencies.FirstOrDefault(c => c.Id == g.Key.CurrencyId)) : null,
                        new Money(g.Sum(x => x.ExpenseAmount.Amount), g.Key.CurrencyId, g.Key.CurrencyCode, g.Key.CurrencySymbol).FormattedAmount,
                        g.Select(d => new ExpenseReportDataDTO(d.ExpenseDate, d.Category.Name, d.Description, d.ExpenseAmount.FormattedAmount)).ToList()
                    )).ToList();


                ExpenseReportDTO expenseReport = new ExpenseReportDTO(request.reportType, request.year, request.month, groupedByCurrency);
                var report = request.reportFormat == Domain.Enums.ReportFormat.Pdf ? _pdfReportGenerator.GenerateAsync(expenseReport) : _excelReportGenerator.GenerateAsync(expenseReport);
                 
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