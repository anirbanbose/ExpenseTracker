using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.ExpenseCategory;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.ExpenseCategory.Queries;

public class GetExpenseCategoryByIdQueryHandler : IRequestHandler<GetExpenseCategoryByIdQuery, Result<ExpenseCategoryDTO?>>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly ILogger<GetExpenseCategoryByIdQueryHandler> _logger;

    public GetExpenseCategoryByIdQueryHandler(IAuthenticatedUserProvider authProvider, IExpenseCategoryRepository expenseCategoryRepository, ILogger<GetExpenseCategoryByIdQueryHandler> logger) 
    {
        _expenseCategoryRepository = expenseCategoryRepository;
        _authProvider = authProvider;
        _logger = logger;
    }

    public async Task<Result<ExpenseCategoryDTO?>> Handle(GetExpenseCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        if (request is null || !request.Id.HasValue)
        {
            return Result<ExpenseCategoryDTO?>.ArgumentNullResult();
        }
        try
        {
            var (currentUser, failureResult) = await _authProvider.GetAuthenticatedUserAsync<Result<ExpenseCategoryDTO?>>(new ResultUserNotAuthenticatedFactory<ExpenseCategoryDTO?>(), cancellationToken);
            if (failureResult != null)
                return failureResult;

            var expenseCategory = await _expenseCategoryRepository.GetExpenseCategoryByIdAsync(request.Id.Value, currentUser!.Id, cancellationToken);
            if (expenseCategory is null)
            {
                return Result<ExpenseCategoryDTO?>.NotFoundResult();
            }
            var dto = ExpenseCategoryDTO.FromDomain(expenseCategory);

            return Result<ExpenseCategoryDTO?>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while handling GetExpenseCategoryByIdQuery for user {_authProvider.CurrentUserName}.");
        }
        return Result<ExpenseCategoryDTO?>.FailureResult("ExpenseCategory.GetExpenseCategoryById");
    }

}