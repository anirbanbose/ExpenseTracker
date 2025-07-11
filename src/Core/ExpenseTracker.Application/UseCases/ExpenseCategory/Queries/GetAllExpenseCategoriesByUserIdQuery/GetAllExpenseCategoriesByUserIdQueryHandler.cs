using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.ExpenseCategory;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.ExpenseCategory.Queries;

public class GetAllExpenseCategoriesByUserIdQueryHandler : IRequestHandler<GetAllExpenseCategoriesByUserIdQuery, Result<List<ExpenseCategoryDTO>>>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly ILogger<GetAllExpenseCategoriesByUserIdQueryHandler> _logger;


    public GetAllExpenseCategoriesByUserIdQueryHandler(IAuthenticatedUserProvider authProvider, IExpenseCategoryRepository expenseCategoryRepository, ILogger<GetAllExpenseCategoriesByUserIdQueryHandler> logger)
    {
        _authProvider = authProvider;
        _expenseCategoryRepository = expenseCategoryRepository;
        _logger = logger;
    }

    public async Task<Result<List<ExpenseCategoryDTO>>> Handle(GetAllExpenseCategoriesByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (currentUser, failureResult) = await _authProvider.GetAuthenticatedUserAsync<Result<List<ExpenseCategoryDTO>>>(new ResultUserNotAuthenticatedFactory<List<ExpenseCategoryDTO>>(), cancellationToken);
            if (failureResult != null)
                return failureResult;

            var expenseCategories = await _expenseCategoryRepository.GetAllExpenseCategoriesByUserIdAsync(currentUser!.Id, cancellationToken);
            if (expenseCategories is null || !expenseCategories.Any())
            {
                return Result<List<ExpenseCategoryDTO>>.NotFoundResult();
            }
            var list = expenseCategories.Select(c => ExpenseCategoryDTO.FromDomain(c)).ToList();

            return Result<List<ExpenseCategoryDTO>>.SuccessResult(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while handling GetAllExpenseCategoriesByUserIdQuery for the user {_authProvider.CurrentUserName}.");
        }
        return Result<List<ExpenseCategoryDTO>>.FailureResult("ExpenseCategory.GetAllExpenseCategories");
    }

}
