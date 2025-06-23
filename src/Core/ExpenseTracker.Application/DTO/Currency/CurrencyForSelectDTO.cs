namespace ExpenseTracker.Application.DTO.Currency;

public record CurrencyForSelectDTO(Guid Value, string Text)
{
    public static CurrencyForSelectDTO FromDomain(Domain.Models.Currency currency)
    {
        return new CurrencyForSelectDTO(currency.Id, $"{currency.Code}{(currency.Symbol != null ? " (" + currency.Symbol + ")" : "")}");
    }
}
