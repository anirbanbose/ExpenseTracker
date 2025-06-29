using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.ExpenseCategory.Commands;

public class AddNewExpenseCategoryCommandHandler : BaseHandler, IRequestHandler<AddNewExpenseCategoryCommand, Result<Guid?>>
{
    private readonly IUserRepository _userRepository;
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddNewExpenseCategoryCommandHandler> _logger;

    public AddNewExpenseCategoryCommandHandler(IUserRepository userRepository, IExpenseCategoryRepository expenseCategoryRepository, IUnitOfWork unitOfWork, ILogger<AddNewExpenseCategoryCommandHandler> logger, IHttpContextAccessor _httpContextAccessor) : base(_httpContextAccessor)
    {
        _userRepository = userRepository;
        _expenseCategoryRepository = expenseCategoryRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid?>> Handle(AddNewExpenseCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result<Guid?>.ArgumentNullResult();
        }
        if (!IsCurrentUserAuthenticated || string.IsNullOrEmpty(CurrentUserName))
        {
            return Result<Guid?>.UserNotAuthenticatedResult();
        }
        try
        {            
            var currentUser = await _userRepository.GetUserByEmailAsync(CurrentUserName, cancellationToken);
            if (currentUser is null || currentUser.Deleted)
            {
                _logger.LogWarning($"User - {CurrentUserName} is not authenticated.");
                return Result<Guid?>.UserNotAuthenticatedResult();
            }
            var expenseCategory = new Domain.Models.ExpenseCategory(
                    request.Name,
                    false,
                    currentUser.Id
                    );

            await _expenseCategoryRepository.AddExpenseCategoryAsync(expenseCategory);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Guid?>.SuccessResult(expenseCategory.Id);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while adding new expense category- {request} for user - {CurrentUserName}.");
        }
        return Result<Guid?>.FailureResult("Expense.AddNewExpenseCategory", "Adding new expense category failed.");
    }
}