using ExpenseTracker.Application.DTO.UserPreference;
using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;

namespace ExpenseTracker.Application.UseCases.UserPreference.Queries;

public record GetUserPreferenceQuery() : IRequest<Result<UserPreferenceDTO>>;