using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Domain.SharedKernel;

public interface IEntity
{
    DateTime CreatedDate { get; }
    DateTime LastModifiedDate { get; }
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void SetTimestampForCreate();
    void SetTimestampForUpdate();
    void AddDomainEvent(IDomainEvent domainEvent);
    void ClearDomainEvents();
}

public abstract class Entity<TId> : IEntity
{
    public virtual TId Id { get; protected set; }
    public bool Deleted { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime LastModifiedDate { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    [NotMapped] public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected Entity(TId id)
    {
        Id = id;
    }
    public override bool Equals(object obj)
    {
        if (!(obj is Entity<TId> other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return Id.Equals(other.Id);
    }

    public static bool operator ==(Entity<TId> a, Entity<TId> b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity<TId> a, Entity<TId> b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return (GetType().ToString() + Id).GetHashCode();
    }
    public void MarkAsDeleted()
    {
        Deleted = true;
    }
    public void Undelete()
    {
        Deleted = false;
    }

    public void SetTimestampForCreate()
    {
        DateTime dt = DateTime.UtcNow;
        CreatedDate = dt;
        LastModifiedDate = dt;
    }

    public void SetTimestampForUpdate()
    {
        LastModifiedDate = DateTime.UtcNow;
    }
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

}

public abstract class Entity : Entity<Guid>
{
    protected Entity(Guid id) : base(id)
    {
    }
}
