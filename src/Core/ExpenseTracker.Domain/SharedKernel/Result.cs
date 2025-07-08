using ExpenseTracker.Domain.SharedKernel.Errors;

namespace ExpenseTracker.Domain.SharedKernel;

public class Result
{
    public bool IsSuccess { get; protected init; } = false;
    public bool IsFailure => !IsSuccess;
    public string ErrorMessage { get; protected init; } = string.Empty;
    public string Code { get; protected init; } = string.Empty;

    protected static string _genericErrorMesssage = "An error occurred while processing your request. Please trye again later.";
    protected Result() { }

    public static Result SuccessResult() => new() { IsSuccess = true };

    public static Result FailureResult(Error error) => new() { ErrorMessage = error.Message, Code = error.Code, IsSuccess = false };
    public static Result FailureResult(string code) => new() { Code = code, ErrorMessage = _genericErrorMesssage, IsSuccess = false };
    public static Result FailureResult(string code, string errorMessage) => new() { Code = code, ErrorMessage = errorMessage, IsSuccess = false };
    public static Result UnauthorizedAccessResult() => FailureResult(Error.PermissionError());
    public static Result UserNotAuthenticatedResult() => FailureResult(Error.UserNotAuthenticatedError());
    public static Result ArgumentNullResult() => new() { IsSuccess = false, Code = "FailureResult.ArgumentNull", ErrorMessage = "Argument is null." };
    public static Result NotFoundResult(string errorMessage) => FailureResult(Error.RecordNotFoundError(errorMessage));
    public static Result NotFoundResult() => FailureResult(Error.RecordNotFoundError(_genericErrorMesssage));
}

public class DomainResult<TValue> : DomainResult
{
    private DomainResult() { }
    public TValue Value { get; private init; }

    public static DomainResult<TValue> SuccessResult(TValue value) => new() { Value = value, IsSuccess = true };
    public static new DomainResult<TValue> DomainValidationFailureResult(Error error) => new() { ErrorMessage = error.Message, Code = error.Code, IsSuccess = false };
    public static new DomainResult<TValue> DomainValidationFailureResult(string errorCode, string errorMessage) => new() { ErrorMessage = errorMessage, Code = errorCode, IsSuccess = false };
}

public class DomainResult : Result
{
    protected DomainResult() { }
    public static new DomainResult SuccessResult() => new() { IsSuccess = true };
    public static DomainResult DomainValidationFailureResult(Error error) => new() { ErrorMessage = error.Message, Code = error.Code, IsSuccess = false };
    public static DomainResult DomainValidationFailureResult(string errorCode, string errorMessage) => new() { ErrorMessage = errorMessage, Code = errorCode, IsSuccess = false };
}

public class Result<TValue> : Result
{
    private Result() { }
    public TValue Value { get; private init; }

    public static implicit operator Result<TValue>(TValue? value) => value is not null ? SuccessResult(value) : FailureResult(Error.NullValue());
    public static Result<TValue> SuccessResult(TValue value) => new() { Value = value, IsSuccess = true };
    public static new Result<TValue> FailureResult(Error error) => new() { ErrorMessage = error.Message, Code = error.Code, IsSuccess = false };
    public static new Result<TValue> FailureResult(string code) => new() { Code = code, ErrorMessage = _genericErrorMesssage, IsSuccess = false };
    public static new Result<TValue> FailureResult(string code, string errorMessage) => new() { Code = code, ErrorMessage = errorMessage, IsSuccess = false };
    public static new Result<TValue> ArgumentNullResult() => new() { IsSuccess = false, Code = "FailureResult.ArgumentNull", ErrorMessage = "Argument is null." };
    public static new Result<TValue> UnauthorizedAccessResult() => FailureResult(Error.PermissionError());
    public static new Result<TValue> UserNotAuthenticatedResult() => FailureResult(Error.UserNotAuthenticatedError());
    public static new Result<TValue> NotFoundResult(string errorMessage) => FailureResult(Error.RecordNotFoundError(errorMessage));
    public static new Result<TValue> NotFoundResult() => FailureResult(Error.RecordNotFoundError(_genericErrorMesssage));
}

public class PagedResult<T> : Result
{
    private PagedResult() { }
    public int TotalCount { get; private set; }
    public int PageIndex { get; private set; }
    public int PageSize { get; private set; }
    public IEnumerable<T> Items { get; private init; } = Enumerable.Empty<T>();
    public static PagedResult<T> SuccessResult(IEnumerable<T> items, int totalCount, int pageIndex, int pageSize) => new() { Items = items, IsSuccess = true, TotalCount = totalCount, PageIndex = pageIndex, PageSize = pageSize };
    public static new PagedResult<T> FailureResult(Error error) => new() { ErrorMessage = error.Message, Code = error.Code, IsSuccess = false };
    public static new PagedResult<T> FailureResult(string code) => new() { Code = code, ErrorMessage = _genericErrorMesssage, IsSuccess = false };
    public static new PagedResult<T> FailureResult(string code, string errorMessage) => new() { Code = code, ErrorMessage = errorMessage, IsSuccess = false };
    public static new PagedResult<T> NotFoundResult(string errorMessage) => FailureResult(Error.RecordNotFoundError(errorMessage));
    public static new PagedResult<T> NotFoundResult() => FailureResult(Error.RecordNotFoundError(_genericErrorMesssage));
    public static new PagedResult<T> UserNotAuthenticatedResult() => FailureResult(Error.UserNotAuthenticatedError());
    public static new PagedResult<T> UnauthorizedAccessResult() => FailureResult(Error.PermissionError());
}


