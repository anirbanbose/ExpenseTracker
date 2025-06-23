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
        var currentUser = await _userRepository.GetUserByEmailAsync(CurrentUserName, cancellationToken);
        if (currentUser is null)
        {
            _logger.LogWarning($"User not authenticated.");
            return Result<ExpenseCategoryDTO?>.UserNotAuthenticatedResult();
        }
        var expenseCategory = await _expenseCategoryRepository.GetExpenseCategoryByIdAsync(request.id, currentUser.Id, cancellationToken);
        if (expenseCategory is null)
        {
            return Result<ExpenseCategoryDTO?>.NotFoundResult("No Expense Category found.");
        }
        var dto = ExpenseCategoryDTO.FromDomain(expenseCategory);

        return Result<ExpenseCategoryDTO?>.SuccessResult(dto);
    }

}