namespace ExpenseTracker.Domain.SharedKernel.Results;

public class Result
{
    public bool IsSuccess { get; protected init; } = false;
    public bool IsFailure => !IsSuccess;
    public string? ErrorMessage { get; protected init; } = null;

    protected static string GenericErrorMesssage = "An error occurred while processing your request. Please trye again later.";
    protected Result() { }

    protected Result(bool isSuccess, string? errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static Result SuccessResult() => new(true, null);
    public static Result FailureResult() => new(false, GenericErrorMesssage);
    public static Result FailureResult(string errorMessage) => new(false, errorMessage);
    public static Result UserNotAuthenticatedResult() => FailureResult("User is not authenticated.");
    public static Result ArgumentNullResult() => FailureResult("Argument is null.");
}



public class Result<TValue> : Result
{
    public TValue? Value { get; private init; }

    private Result(TValue value) : base(true, null)
    {
        Value = value;
    }

    private Result(string errorMessage) : base(false, errorMessage)
    {
        Value = default;
    }

    public static Result<TValue> SuccessResult(TValue value) => new(value);
    public static new Result<TValue> FailureResult(string errorMessage) => new(errorMessage);
    public static new Result<TValue> FailureResult() => new(GenericErrorMesssage);
    public static new Result<TValue> ArgumentNullResult() => FailureResult("Argument is null.");
    public static new Result<TValue> UserNotAuthenticatedResult() => FailureResult("User is not authenticated.");
}




