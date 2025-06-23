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
        try
        {
            if (!IsCurrentUserAuthenticated || string.IsNullOrEmpty(CurrentUserName))
            {
                return Result<List<ExpenseCategoryDTO>>.UserNotAuthenticatedResult();
            }
            var currentUser = await _userRepository.GetUserByEmailAsync(CurrentUserName, cancellationToken);
            if (currentUser is null)
            {
                _logger.LogWarning($"User not authenticated.");
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
            _logger.LogError(ex, "An error occurred while handling GetAllExpenseCategoriesByUserIdQuery.");
        }
        return Result<List<ExpenseCategoryDTO>>.FailureResult("ExpenseCategory.UnknownException", "An error occurred while processing your request.");
    }

}
