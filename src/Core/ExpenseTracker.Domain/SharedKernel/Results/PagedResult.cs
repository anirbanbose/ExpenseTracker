namespace ExpenseTracker.Domain.SharedKernel.Results;

public class PagedResult<T> : Result
{
    public int? TotalCount { get; private set; } = null;
    public int? PageIndex { get; private set; } = null;
    public int? PageSize { get; private set; } = null;
    public IEnumerable<T>? Items { get; private init; } = Enumerable.Empty<T>();

    private PagedResult(IEnumerable<T> items, int totalCount, int pageIndex, int pageSize) : base(true, null)
    {
        Items = items;
        TotalCount = totalCount;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    private PagedResult(string errorMessage) : base(false, errorMessage)
    {
        Items = default;
        TotalCount = null;
        PageIndex = null;
        PageSize = null;
    }

    public static PagedResult<T> SuccessResult(IEnumerable<T> items, int totalCount, int pageIndex, int pageSize) => new(items, totalCount, pageIndex, pageSize);
    public static new PagedResult<T> FailureResult() => new(GenericErrorMesssage);
    public static new PagedResult<T> FailureResult(string errorMessage) => new(errorMessage);
    public static new PagedResult<T> UserNotAuthenticatedResult() => FailureResult("User is not authenticated.");
}