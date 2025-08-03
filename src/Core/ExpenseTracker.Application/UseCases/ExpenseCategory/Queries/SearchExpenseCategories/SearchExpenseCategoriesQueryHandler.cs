using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.ExpenseCategory;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.ExpenseCategory.Queries;

public class SearchExpenseCategoriesQueryHandler : IRequestHandler<SearchExpenseCategoriesQuery, PagedResult<ExpenseCategoryDTO>>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly ILogger<SearchExpenseCategoriesQueryHandler> _logger;


    public SearchExpenseCategoriesQueryHandler(IAuthenticatedUserProvider authProvider, IExpenseCategoryRepository expenseCategoryRepository, ILogger<SearchExpenseCategoriesQueryHandler> logger) 
    {
        _expenseCategoryRepository = expenseCategoryRepository;
        _authProvider = authProvider;
        _logger = logger;
    }

    public async Task<PagedResult<ExpenseCategoryDTO>> Handle(SearchExpenseCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (currentUser, failureResult) = await _authProvider.GetAuthenticatedUserAsync<PagedResult<ExpenseCategoryDTO>>(new PagedResultUserNotAuthenticatedFactory<ExpenseCategoryDTO>(), cancellationToken);
            if (failureResult != null)
                return failureResult;

            var expenseCategoryResult = await _expenseCategoryRepository.SearchUserExpenseCategoriesAsync(request.Search, currentUser!.Id, request.PageIndex, request.PageSize, request.Order, request.IsAscendingSort, cancellationToken);
            if (expenseCategoryResult.Items is null)
            {
                return PagedResult<ExpenseCategoryDTO>.FailureResult();
            }
            
            List<ExpenseCategoryDTO> dtoList = new List<ExpenseCategoryDTO>();
            expenseCategoryResult.Items.ToList().ForEach(category =>
            {
                dtoList.Add(ExpenseCategoryDTO.FromDomain(category));
            });
            return PagedResult<ExpenseCategoryDTO>.SuccessResult(dtoList, expenseCategoryResult.TotalCount, request.PageIndex, request.PageSize);
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while handling SearchExpenseCategoriesQuery for user - {_authProvider.CurrentUserName}.");
        }        

        return PagedResult<ExpenseCategoryDTO>.FailureResult();

    }

}