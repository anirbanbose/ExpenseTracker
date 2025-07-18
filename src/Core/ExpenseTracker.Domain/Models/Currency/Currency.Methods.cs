﻿using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Domain.Models;

public partial class Currency : Entity<CurrencyId>
{
    private Currency(CurrencyId id) : base(id) { }

    public Currency(string code, string name, string? symbol = default) : base(CurrencyId.Create())
    {
        Name = name;
        Code = code;
        Symbol = symbol;
    }
    public static DomainResult<Currency> Create(string code, string name, string? symbol = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return DomainResult<Currency>.DomainValidationFailureResult("DomainError.Currency.NullArgumentError", "Currency Code cannot be empty.");
        if (string.IsNullOrWhiteSpace(name))
            return DomainResult<Currency>.DomainValidationFailureResult("DomainError.Currency.NullArgumentError", "Currency Name cannot be empty.");
        return DomainResult <Currency>.SuccessResult(new Currency(code, name, symbol));
    }
    public DomainResult Update(string code, string name, string? symbol = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return DomainResult.DomainValidationFailureResult("DomainError.Currency.NullArgumentError", "Currency Code cannot be empty.");
        if (string.IsNullOrWhiteSpace(name))
            return DomainResult.DomainValidationFailureResult("DomainError.Currency.NullArgumentError", "Currency Name cannot be empty.");
        Code = code;
        Symbol = symbol;
        Name = name;
        return DomainResult.SuccessResult();
    }
}