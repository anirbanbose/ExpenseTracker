
using System.Linq.Expressions;

namespace ExpenseTracker.Domain.Enums;


public enum CurrencyListOrder
{
    Name = 0,
    Code,
    Symbol,
}


public static class CurrencyListOrderOrderExtension
{
    public static Expression<Func<Models.Currency, object?>> ToOrderExpression(this CurrencyListOrder order) => order switch
    {
        CurrencyListOrder.Name => d => d.Name,
        CurrencyListOrder.Code => d => d.Code,
        CurrencyListOrder.Symbol => d => d.Symbol,
        _ => d => d.Name,
    };
}