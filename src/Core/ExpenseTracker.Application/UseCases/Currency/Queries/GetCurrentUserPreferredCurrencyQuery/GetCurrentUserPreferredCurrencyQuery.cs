using ExpenseTracker.Application.DTO.Currency;
using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;

namespace ExpenseTracker.Application.UseCases.Currency.Queries;

public record GetCurrentUserPreferredCurrencyQuery() : IRequest<Result<CurrencyDTO>>;
