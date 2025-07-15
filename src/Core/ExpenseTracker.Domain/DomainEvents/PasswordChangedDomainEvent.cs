using ExpenseTracker.Domain.SharedKernel;


namespace ExpenseTracker.Domain.DomainEvents;

public sealed record PasswordChangedDomainEvent(string UserEmail, string UserFullName) : IDomainEvent;