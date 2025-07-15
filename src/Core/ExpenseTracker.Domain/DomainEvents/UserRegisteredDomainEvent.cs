using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Domain.DomainEvents;

public sealed record UserRegisteredDomainEvent(string UserEmail, string UserFullName) : IDomainEvent;