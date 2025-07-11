namespace ExpenseTracker.Domain.SharedKernel;

public interface IUserNotAuthenticatedResultFactory<T>
{
    T? UserNotAuthenticatedResult();
}

public class ResultUserNotAuthenticatedFactory : IUserNotAuthenticatedResultFactory<Result>
{
    public Result? UserNotAuthenticatedResult() => Result.UserNotAuthenticatedResult();
}

public class ResultUserNotAuthenticatedFactory<T> : IUserNotAuthenticatedResultFactory<Result<T>>
{
    public Result<T>? UserNotAuthenticatedResult() => Result<T>.UserNotAuthenticatedResult();
}

public class PagedResultUserNotAuthenticatedFactory<T> : IUserNotAuthenticatedResultFactory<PagedResult<T>>
{
    public PagedResult<T>? UserNotAuthenticatedResult() => PagedResult<T>.UserNotAuthenticatedResult();
}
