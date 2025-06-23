using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;
using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.User;

namespace ExpenseTracker.Application.UseCases.User.Queries;

public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<LoggedInUserDTO>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<LoginQueryHandler> _logger;
    private readonly IAuthManager _authManager;

    public LoginQueryHandler(IUserRepository userRepository, ILogger<LoginQueryHandler> logger, IAuthManager authManager)
    {
        _userRepository = userRepository;
        _logger = logger;
        _authManager = authManager;
    }

    public async Task<Result<LoggedInUserDTO>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is not null)
            {
                var user = await _userRepository.GetUserByEmailAsync(request.Email, cancellationToken);
                if (user is null || user.Deleted)
                {
                    return Result<LoggedInUserDTO>.FailureResult("Login.UnauthorizedUser", "Login failed.");
                }
                var passwordVerificationResult = user.VerifyPassword(request.Password);
                if (passwordVerificationResult.IsFailure)
                {
                    return Result<LoggedInUserDTO>.FailureResult("Login.UnauthorizedUser", "Login failed.");
                }
                var loggedInUser = LoggedInUserDTO.FromDomain(user);
                if (_authManager.GenerateTokensAndSetCookies(loggedInUser, request.RememberMe))
                {
                    return Result<LoggedInUserDTO>.SuccessResult(loggedInUser);
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex.Message, ex);
        }
        
        return Result<LoggedInUserDTO>.FailureResult("Login.UnauthorizedUser", "There was an issue while signing you in. Please try again later.");
    }
}
