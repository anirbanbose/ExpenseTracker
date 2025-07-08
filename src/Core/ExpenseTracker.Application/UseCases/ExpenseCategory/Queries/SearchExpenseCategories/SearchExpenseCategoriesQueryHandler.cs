using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.ExpenseCategory;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.Persistence.SearchModels;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.ExpenseCategory.Queries;

public class SearchExpenseCategoriesQueryHandler : BaseHandler, IRequestHandler<SearchExpenseCategoriesQuery, PagedResult<ExpenseCategoryDTO>>
{
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SearchExpenseCategoriesQueryHandler> _logger;


    public SearchExpenseCategoriesQueryHandler(ICurrentUserManager currentUserManager, IExpenseCategoryRepository expenseCategoryRepository, IUserRepository userRepository, ILogger<SearchExpenseCategoriesQueryHandler> logger) : base(currentUserManager)
    {
        _expenseCategoryRepository = expenseCategoryRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<PagedResult<ExpenseCategoryDTO>> Handle(SearchExpenseCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {            
            var (currentUser, failureResult) = await GetAuthenticatedUserAsync(_userRepository, _logger, cancellationToken, new PagedResultUserNotAuthenticatedFactory<ExpenseCategoryDTO>());
            if (failureResult != null)
                return failureResult;

            var expenseCategoryResult = await _expenseCategoryRepository.SearchUserExpenseCategoriesAsync(ExpenseCategorySearchModel.Create(request.Search), currentUser.Id, request.PageIndex, request.PageSize, request.Order, request.IsAscendingSort, cancellationToken);

            if (expenseCategoryResult.IsSuccess && expenseCategoryResult.Items.Any())
            {
                List<ExpenseCategoryDTO> dtoList = new List<ExpenseCategoryDTO>();
                expenseCategoryResult.Items.ToList().ForEach(category =>
                {
                    dtoList.Add(ExpenseCategoryDTO.FromDomain(category));
                });
                return PagedResult<ExpenseCategoryDTO>.SuccessResult(dtoList, expenseCategoryResult.TotalCount, expenseCategoryResult.PageIndex, expenseCategoryResult.PageSize);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while handling SearchExpenseCategoriesQuery for user - {CurrentUserName}.");
        }        

        return PagedResult<ExpenseCategoryDTO>.FailureResult("ExpenseCategory.SearchExpenseCategories");

    }

}