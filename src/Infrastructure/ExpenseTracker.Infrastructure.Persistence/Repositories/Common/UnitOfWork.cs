using ExpenseTracker.Domain.Persistence;

namespace ExpenseTracker.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task CommitAsync(CancellationToken cancellationToken)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }

    public ValueTask RollBackAsync()
    {
        return _db.DisposeAsync();
    }
}
