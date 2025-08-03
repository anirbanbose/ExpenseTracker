using ExpenseTracker.Application.DTO.Currency;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;

namespace ExpenseTracker.Application.UseCases.Currency.Queries;

public record SearchCurrenciesQuery(string? search, int pageIndex, int pageSize, CurrencyListOrder order, bool isAscendingSort) : IRequest<PagedResult<CurrencyDTO>>;
