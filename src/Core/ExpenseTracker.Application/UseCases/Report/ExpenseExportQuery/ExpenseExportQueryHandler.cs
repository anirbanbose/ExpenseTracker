using ExpenseTracker.Application.Contracts.Report;
using ExpenseTracker.Application.DTO.Report;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.Persistence.SearchModels;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.Report;

public class ExpenseExportQueryHandler : BaseHandler, IRequestHandler<ExpenseExportQuery, Result<byte[]>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ExpenseExportQueryHandler> _logger;
    private IExcelReportGenerator<ExpenseExportDTO> _excelExportGenerator;
    private IPdfReportGenerator<ExpenseExportDTO> _pdfExportGenerator;

    public ExpenseExportQueryHandler(IHttpContextAccessor httpContextAccessor, IExpenseRepository expenseRepository, IUserRepository userRepository, IExpenseCategoryRepository expenseCategoryRepository, IExcelReportGenerator<ExpenseExportDTO> excelExportGenerator, IPdfReportGenerator<ExpenseExportDTO> pdfExportGenerator, ILogger<ExpenseExportQueryHandler> logger) : base(httpContextAccessor)
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
        if (!IsCurrentUserAuthenticated || string.IsNullOrEmpty(CurrentUserName))
        {
            return Result<byte[]>.UserNotAuthenticatedResult();
        }
        try
        {
            var currentUser = await _userRepository.GetUserByEmailAsync(CurrentUserName, cancellationToken);
            if (currentUser is null)
            {
                _logger.LogWarning("User not authenticated.");
                return Result<byte[]>.UserNotAuthenticatedResult();
            }
            var expenseResult = await _expenseRepository.SearchExpensesAsync(ExpenseSearchModel.Create(request.search, request.expenseCategoryId, request.currencyId, request.startDate, request.endDate), currentUser.Id, request.order, request.IsAscendingSort, cancellationToken);


            if (expenseResult is not null)
            {
                ExpenseExportDTO expenseExportDTO = new ExpenseExportDTO();
                expenseExportDTO.SearchText = string.IsNullOrEmpty(request.search) ? request.search : null;
                if (request.expenseCategoryId.HasValue)
                {
                    var expCategory = await _expenseCategoryRepository.GetExpenseCategoryByIdAsync(request.expenseCategoryId.Value, cancellationToken);
                    if (expCategory is not null)
                    {
                        expenseExportDTO.Category = expCategory.Name;
                    }
                }
                expenseExportDTO.StartDate = request.startDate;
                expenseExportDTO.EndDate = request.endDate;

                expenseResult.ToList().ForEach(category =>
                {
                    expenseExportDTO.ReportData.Add(ExpenseReportDTO.FromDomain(category));
                });
                var report = request.reportFormat == Domain.Enums.ReportFormat.Pdf ? _pdfExportGenerator.GenerateAsync(expenseExportDTO) : _excelExportGenerator.GenerateAsync(expenseExportDTO);

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