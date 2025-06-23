using ExpenseTracker.Application.DTO.ExpenseCategory;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.Persistence.SearchModels;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.ExpenseCategory.Queries;

public class SearchExpenseCategoriesQueryHandler : BaseHandler, IRequestHandler<SearchExpenseCategoriesQuery, PagedResult<ExpenseCategoryDTO>>
{
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SearchExpenseCategoriesQueryHandler> _logger;


    public SearchExpenseCategoriesQueryHandler(IHttpContextAccessor httpContextAccessor, IExpenseCategoryRepository expenseCategoryRepository, IUserRepository userRepository, ILogger<SearchExpenseCategoriesQueryHandler> logger) : base(httpContextAccessor)
    {
        _expenseCategoryRepository = expenseCategoryRepository;
        _userRepository = userRepository;
        _logger = logger;
    }


    public async Task<PagedResult<ExpenseCategoryDTO>> Handle(SearchExpenseCategoriesQuery request, CancellationToken cancellationToken)
    {
        if (!IsCurrentUserAuthenticated || string.IsNullOrEmpty(CurrentUserName))
        {
            return PagedResult<ExpenseCategoryDTO>.UserNotAuthenticatedResult();
        }
        var currentUser = await _userRepository.GetUserByEmailAsync(CurrentUserName, cancellationToken);
        if (currentUser is null)
        {
            _logger.LogWarning($"User not authenticated.");
            return PagedResult<ExpenseCategoryDTO>.UserNotAuthenticatedResult();
        }
        var expenseCategoryResult = await _expenseCategoryRepository.SearchExpenseCategoriesAsync(ExpenseCategorySearchModel.Create(request.search), currentUser.Id, request.PageIndex, request.PageSize, request.Order, request.IsAscendingSort, cancellationToken);

        if (expenseCategoryResult.IsSuccess && expenseCategoryResult.Items.Any())
        {
            List<ExpenseCategoryDTO> dtoList = new List<ExpenseCategoryDTO>();
            expenseCategoryResult.Items.ToList().ForEach(category =>
            {
                dtoList.Add(ExpenseCategoryDTO.FromDomain(category));
            });
            return PagedResult<ExpenseCategoryDTO>.SuccessResult(dtoList, expenseCategoryResult.TotalCount, expenseCategoryResult.PageIndex, expenseCategoryResult.PageSize);
        }

        return PagedResult<ExpenseCategoryDTO>.FailureResult("ExpenseCategory.SearchExpenseCategories", "Couldn't fetch the expense category list.");

    }

}