namespace ExpenseTracker.Domain.Models;

public record CurrencyId(Guid Value)
{
    public static implicit operator Guid(CurrencyId id) => id.Value;
    public static explicit operator CurrencyId(Guid value) => new CurrencyId(value);
    public static CurrencyId Create() => new CurrencyId(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
