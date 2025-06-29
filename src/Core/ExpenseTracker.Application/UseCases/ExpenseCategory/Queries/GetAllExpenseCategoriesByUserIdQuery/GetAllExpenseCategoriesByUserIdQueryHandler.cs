using ExpenseTracker.Application.DTO.ExpenseCategory;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.ExpenseCategory.Queries;

public class GetAllExpenseCategoriesByUserIdQueryHandler : BaseHandler, IRequestHandler<GetAllExpenseCategoriesByUserIdQuery, Result<List<ExpenseCategoryDTO>>>
{
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetAllExpenseCategoriesByUserIdQueryHandler> _logger;


    public GetAllExpenseCategoriesByUserIdQueryHandler(IHttpContextAccessor httpContextAccessor, IExpenseCategoryRepository expenseCategoryRepository, IUserRepository userRepository, ILogger<GetAllExpenseCategoriesByUserIdQueryHandler> logger) : base(httpContextAccessor)
    {
        _expenseCategoryRepository = expenseCategoryRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<List<ExpenseCategoryDTO>>> Handle(GetAllExpenseCategoriesByUserIdQuery request, CancellationToken cancellationToken)
    {
        if (!IsCurrentUserAuthenticated || string.IsNullOrEmpty(CurrentUserName))
        {
            return Result<List<ExpenseCategoryDTO>>.UserNotAuthenticatedResult();
        }
        try
        {            
            var currentUser = await _userRepository.GetUserByEmailAsync(CurrentUserName, cancellationToken);
            if (currentUser is null || currentUser.Deleted)
            {
                _logger.LogWarning($"User - {CurrentUserName} is not authenticated.");
                return Result<List<ExpenseCategoryDTO>>.UserNotAuthenticatedResult();
            }
            var expenseCategories = await _expenseCategoryRepository.GetAllExpenseCategoriesByUserIdAsync(currentUser.Id, cancellationToken);
            if (expenseCategories is null || !expenseCategories.Any())
            {
                return Result<List<ExpenseCategoryDTO>>.NotFoundResult("No Expense Category found.");
            }
            var list = expenseCategories.Select(c => ExpenseCategoryDTO.FromDomain(c)).ToList();

            return Result<List<ExpenseCategoryDTO>>.SuccessResult(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while handling GetAllExpenseCategoriesByUserIdQuery for the user {CurrentUserName}.");
        }
        return Result<List<ExpenseCategoryDTO>>.FailureResult("ExpenseCategory.GetAllExpenseCategories", "An error occurred while fetching the list of expense categories.");
    }

}
