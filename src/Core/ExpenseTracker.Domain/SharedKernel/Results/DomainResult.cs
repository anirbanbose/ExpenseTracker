namespace ExpenseTracker.Domain.SharedKernel.Results;

public class DomainResult : Result
{
    public string? ErrorCode { get; protected init; } = null;
    protected DomainResult(bool isSuceess, string? errorCode, string? errorMessage) : base(isSuceess, errorMessage)
    {
        ErrorCode = errorCode;
    }
    public static new DomainResult SuccessResult() => new(true, null, null);
    public static DomainResult DomainValidationFailureResult(string errorCode, string errorMessage) => new(false, errorCode, errorMessage);
}

public class DomainResult<TValue> : DomainResult
{
    public TValue? Value { get; private init; }
    private DomainResult(TValue value) : base(true, null, null)
    {
        Value = value;
    }
    private DomainResult(string errorCode, string errorMessage) : base(false, errorCode, errorMessage)
    {
        Value = default;
    }
    public static DomainResult<TValue> SuccessResult(TValue value) => new(value);
    public static new DomainResult<TValue> DomainValidationFailureResult(string errorCode, string errorMessage) => new(errorCode, errorMessage);
}