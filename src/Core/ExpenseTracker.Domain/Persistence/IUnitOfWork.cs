namespace ExpenseTracker.Domain.Persistence;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken);
    ValueTask RollBackAsync();
}
