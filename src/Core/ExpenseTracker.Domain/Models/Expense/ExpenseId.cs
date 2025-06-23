namespace ExpenseTracker.Domain.Models;

public record ExpenseId(Guid Value)
{
    public static implicit operator Guid(ExpenseId id) => id.Value;
    public static explicit operator ExpenseId(Guid value) => new ExpenseId(value);
    public static ExpenseId Create() => new ExpenseId(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
