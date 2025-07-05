namespace ExpenseTracker.Application.DTO.Currency;

public record CurrencyDTO(Guid Id, string Code, string? Symbol, string Name)
{
    public static CurrencyDTO FromDomain(Domain.Models.Currency currency)
    {
        return new CurrencyDTO(currency.Id, currency.Code, currency.Symbol, currency.Name);
    }

    public string LongCurrencyPrefix
    {
        get
        {
            return $"{Code} {(!string.IsNullOrEmpty(Symbol) ? Symbol : "")}";
        }
    }

    public string ShortCurrencyPrefix
    {
        get
        {
            return !string.IsNullOrEmpty(Symbol) ? Symbol : Code;
        }
    }
}
