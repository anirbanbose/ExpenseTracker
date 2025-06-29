using ExpenseTracker.Application.DTO.ExpenseCategory;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.ExpenseCategory.Queries;

public class GetExpenseCategoryByIdQueryHandler : BaseHandler, IRequestHandler<GetExpenseCategoryByIdQuery, Result<ExpenseCategoryDTO?>>
{
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetExpenseCategoryByIdQueryHandler> _logger;

    public GetExpenseCategoryByIdQueryHandler(IHttpContextAccessor httpContextAccessor, IExpenseCategoryRepository expenseCategoryRepository, IUserRepository userRepository, ILogger<GetExpenseCategoryByIdQueryHandler> logger) : base(httpContextAccessor)
    {
        _expenseCategoryRepository = expenseCategoryRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<ExpenseCategoryDTO?>> Handle(GetExpenseCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        if (!IsCurrentUserAuthenticated || string.IsNullOrEmpty(CurrentUserName))
        {
            return Result<ExpenseCategoryDTO?>.UserNotAuthenticatedResult();
        }
        if (request is null || !request.Id.HasValue)
        {
            return Result<ExpenseCategoryDTO?>.ArgumentNullResult();
        }
        try
        {
            var currentUser = await _userRepository.GetUserByEmailAsync(CurrentUserName, cancellationToken);
            if (currentUser is null || currentUser.Deleted)
            {
                _logger.LogWarning($"User - {CurrentUserName} is not authenticated.");
                return Result<ExpenseCategoryDTO?>.UserNotAuthenticatedResult();
            }
            var expenseCategory = await _expenseCategoryRepository.GetExpenseCategoryByIdAsync(request.Id.Value, currentUser.Id, cancellationToken);
            if (expenseCategory is null)
            {
                return Result<ExpenseCategoryDTO?>.NotFoundResult("No Expense Category found.");
            }
            var dto = ExpenseCategoryDTO.FromDomain(expenseCategory);

            return Result<ExpenseCategoryDTO?>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while handling GetExpenseCategoryByIdQuery for user {CurrentUserName}.");
        }
        return Result<ExpenseCategoryDTO?>.FailureResult("ExpenseCategory.GetExpenseCategoryById", "Couldn't fetch the expense category data.");
    }

}