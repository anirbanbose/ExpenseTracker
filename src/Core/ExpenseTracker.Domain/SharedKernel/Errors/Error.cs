using ExpenseTracker.Domain.Utils;

namespace ExpenseTracker.Domain.SharedKernel.Errors;

public record Error
{

    public string Code { get; }

    public string Message { get; }

    public ErrorType Type { get; } = ErrorType.Failure;

    public Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    public static Error None() => new(string.Empty, string.Empty, ErrorType.NoError);
    public static Error NullValue() => new("General.Null", "Null value was provided", ErrorType.Failure);
    public static Error RecordNotFoundError(string message) => new(Constants.RecordNotFoundErrorCode, message, ErrorType.NotFound);

    public static Error ValidationError(string message) => new(Constants.ValidationErrorCode, message, ErrorType.Validation);

    public static Error UserNotAuthenticatedError() => new(Constants.AuthenticationErrorCode, "User is not authenticated.", ErrorType.AuthenticationError);

    public static Error PermissionError() => new(Constants.UnauthorizedErrorCode, "User is not authorized for this operation.", ErrorType.UnauthorizedError);

}