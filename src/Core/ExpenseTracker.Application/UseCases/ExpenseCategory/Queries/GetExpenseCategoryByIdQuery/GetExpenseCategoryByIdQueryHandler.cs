using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.ExpenseCategory;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.ExpenseCategory.Queries;

public class GetExpenseCategoryByIdQueryHandler : BaseHandler, IRequestHandler<GetExpenseCategoryByIdQuery, Result<ExpenseCategoryDTO?>>
{
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetExpenseCategoryByIdQueryHandler> _logger;

    public GetExpenseCategoryByIdQueryHandler(ICurrentUserManager currentUserManager, IExpenseCategoryRepository expenseCategoryRepository, IUserRepository userRepository, ILogger<GetExpenseCategoryByIdQueryHandler> logger) : base(currentUserManager)
    {
        _expenseCategoryRepository = expenseCategoryRepository;
        _userRepository = userRepository;
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
            var (currentUser, failureResult) = await GetAuthenticatedUserAsync(_userRepository, _logger, cancellationToken, new ResultUserNotAuthenticatedFactory<ExpenseCategoryDTO?>());
            if (failureResult != null)
                return failureResult;

            var expenseCategory = await _expenseCategoryRepository.GetExpenseCategoryByIdAsync(request.Id.Value, currentUser.Id, cancellationToken);
            if (expenseCategory is null)
            {
                return Result<ExpenseCategoryDTO?>.NotFoundResult();
            }
            var dto = ExpenseCategoryDTO.FromDomain(expenseCategory);

            return Result<ExpenseCategoryDTO?>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while handling GetExpenseCategoryByIdQuery for user {CurrentUserName}.");
        }
        return Result<ExpenseCategoryDTO?>.FailureResult("ExpenseCategory.GetExpenseCategoryById");
    }

}