using ExpenseTracker.Application.DTO.User;
using ExpenseTracker.Domain.Models;
using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.User.Queries;

public class LoggedinUserQueryHandler : BaseHandler, IRequestHandler<LoggedinUserQuery, Result<LoggedInUserDTO>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<LoggedinUserQueryHandler> _logger;
    public LoggedinUserQueryHandler(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, ILogger<LoggedinUserQueryHandler> logger) : base(httpContextAccessor)
    {
        _userRepository = userRepository;
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
            if (!IsCurrentUserAuthenticated || string.IsNullOrEmpty(CurrentUserName))
            {
                return Result<LoggedInUserDTO>.UserNotAuthenticatedResult();
            }
            var currentUser = await _userRepository.GetUserByEmailAsync(CurrentUserName, cancellationToken, true);
            if (currentUser is null)
            {
                _logger.LogWarning($"User not authenticated.");
                return Result<LoggedInUserDTO>.UserNotAuthenticatedResult();
            }
            var loggedInUser = LoggedInUserDTO.FromDomain(currentUser);

            return Result<LoggedInUserDTO>.SuccessResult(loggedInUser);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex.Message, ex);
        }
        return Result<LoggedInUserDTO>.FailureResult("Account.LoggedinUser", "Loggedin User failed.");
    }
}