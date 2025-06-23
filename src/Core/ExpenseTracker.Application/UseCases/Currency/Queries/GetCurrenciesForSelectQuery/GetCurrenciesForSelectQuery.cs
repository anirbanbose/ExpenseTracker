using ExpenseTracker.Application.DTO.Currency;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;

namespace ExpenseTracker.Application.UseCases.Currency.Queries;
public record GetCurrenciesForSelectQuery() : IRequest<Result<List<CurrencyForSelectDTO>>>;
