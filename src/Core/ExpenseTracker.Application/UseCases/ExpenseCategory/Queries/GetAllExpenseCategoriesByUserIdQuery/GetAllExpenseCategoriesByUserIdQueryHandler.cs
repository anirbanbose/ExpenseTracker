using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.ExpenseCategory;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.ExpenseCategory.Queries;

public class GetAllExpenseCategoriesByUserIdQueryHandler : BaseHandler, IRequestHandler<GetAllExpenseCategoriesByUserIdQuery, Result<List<ExpenseCategoryDTO>>>
{
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetAllExpenseCategoriesByUserIdQueryHandler> _logger;


    public GetAllExpenseCategoriesByUserIdQueryHandler(ICurrentUserManager currentUserManager, IExpenseCategoryRepository expenseCategoryRepository, IUserRepository userRepository, ILogger<GetAllExpenseCategoriesByUserIdQueryHandler> logger) : base(currentUserManager)
    {
        _expenseCategoryRepository = expenseCategoryRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<List<ExpenseCategoryDTO>>> Handle(GetAllExpenseCategoriesByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (currentUser, failureResult) = await GetAuthenticatedUserAsync(_userRepository, _logger, cancellationToken, new ResultUserNotAuthenticatedFactory<List<ExpenseCategoryDTO>>());
            if (failureResult != null)
                return failureResult;

            var expenseCategories = await _expenseCategoryRepository.GetAllExpenseCategoriesByUserIdAsync(currentUser.Id, cancellationToken);
            if (expenseCategories is null || !expenseCategories.Any())
            {
                return Result<List<ExpenseCategoryDTO>>.NotFoundResult();
            }
            var list = expenseCategories.Select(c => ExpenseCategoryDTO.FromDomain(c)).ToList();

            return Result<List<ExpenseCategoryDTO>>.SuccessResult(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while handling GetAllExpenseCategoriesByUserIdQuery for the user {CurrentUserName}.");
        }
        return Result<List<ExpenseCategoryDTO>>.FailureResult("ExpenseCategory.GetAllExpenseCategories");
    }

}
