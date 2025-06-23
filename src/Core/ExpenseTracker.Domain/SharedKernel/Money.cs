
namespace ExpenseTracker.Domain.SharedKernel;

public record Money(decimal Amount, Guid CurrencyId, string CurrencyCode, string? CurrencySymbol)
{
    private const int DefaultFractionDigits = 2;

    public string FormattedAmount =>
        $"{CurrencyCode} {(!string.IsNullOrEmpty(CurrencySymbol) ? CurrencySymbol : "")}{decimal.Round(Amount, DefaultFractionDigits, MidpointRounding.ToEven):N2}";

    public override string ToString()
    {
        return FormattedAmount;
    }
}

public static class MoneyExtensions
{
    public static Money Add(this Money money1, Money money2)
    {
        if (!money1.CurrencyId.Equals(money2.CurrencyId))
            throw new InvalidOperationException("Cannot add two Money objects with different currencies.");
        return new Money(money1.Amount + money2.Amount, money1.CurrencyId, money1.CurrencyCode, money1.CurrencySymbol);
    }
    public static Money Subtract(this Money money1, Money money2)
    {
        if (!money1.CurrencyId.Equals(money2.CurrencyId))
            throw new InvalidOperationException("Cannot subtract two Money objects with different currencies.");
        return new Money(money1.Amount - money2.Amount, money1.CurrencyId, money1.CurrencyCode, money1.CurrencySymbol);
    }
}