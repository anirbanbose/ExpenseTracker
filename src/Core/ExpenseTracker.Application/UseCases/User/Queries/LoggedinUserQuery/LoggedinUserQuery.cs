using ExpenseTracker.Application.DTO.User;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;

namespace ExpenseTracker.Application.UseCases.User.Queries;

public record LoggedinUserQuery() : IRequest<Result<LoggedInUserDTO>>;
