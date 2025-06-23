using ExpenseTracker.Application.DTO.Profile;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;

namespace ExpenseTracker.Application.UseCases.Profile.Queries;

public record GetUserProfileQuery() : IRequest<Result<UserProfileDTO>>;