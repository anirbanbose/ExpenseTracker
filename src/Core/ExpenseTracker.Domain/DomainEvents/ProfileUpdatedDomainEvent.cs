

using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Domain.DomainEvents;

public sealed record ProfileUpdatedDomainEvent(string UserEmail, string OldFullName, string NewFullName) : IDomainEvent;