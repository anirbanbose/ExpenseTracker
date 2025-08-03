namespace ExpenseTracker.Domain.Enums;

public enum ErrorType
{
    Failure = 1,
    Validation,
    NotFound,
    AuthenticationError,
    UnauthorizedError,
    DomainError,
    BusinessError,
    InfrastructureError,
    ApplicationError,
    ServiceError,
    UnknownError,
    DataError,
    RepositoryError,
    CacheError,
    NetworkError,
    SecurityError,
    ConfigurationError,
    TimeoutError,
    DependencyError,
    RateLimitError,
    NoError
}
