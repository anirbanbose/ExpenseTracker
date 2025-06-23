namespace ExpenseTracker.Domain.Models;

public record UserId(Guid Value)
{
    public static implicit operator Guid(UserId id) => id.Value;
    public static explicit operator UserId(Guid value) => new UserId(value);
    public static UserId Create() => new UserId(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
