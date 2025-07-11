using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.User;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.User.Queries;

public class LoggedinUserQueryHandler : IRequestHandler<LoggedinUserQuery, Result<LoggedInUserDTO>>
{
    private readonly IAuthenticatedUserProvider _authProvider;
    private readonly ILogger<LoggedinUserQueryHandler> _logger;
    public LoggedinUserQueryHandler(IAuthenticatedUserProvider authProvider, ILogger<LoggedinUserQueryHandler> logger) 
    {
        _authProvider = authProvider;
        _logger = logger;
    }

    public async Task<Result<LoggedInUserDTO>> Handle(LoggedinUserQuery request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result<LoggedInUserDTO>.ArgumentNullResult();
        }
        try
        {
            var (currentUser, failureResult) = await _authProvider.GetAuthenticatedUserAsync<Result<LoggedInUserDTO>>(new ResultUserNotAuthenticatedFactory<LoggedInUserDTO>(), cancellationToken);
            if (failureResult != null)
                return failureResult;

            var loggedInUser = LoggedInUserDTO.FromDomain(currentUser!);

            return Result<LoggedInUserDTO>.SuccessResult(loggedInUser);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while getting the LoggedinUser data for the user: {_authProvider.CurrentUserName}.");
        }
        return Result<LoggedInUserDTO>.FailureResult("Account.LoggedinUser");
    }
}