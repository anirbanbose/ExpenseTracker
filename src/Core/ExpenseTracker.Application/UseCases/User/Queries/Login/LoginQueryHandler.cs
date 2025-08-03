using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.User;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.User.Queries;

public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<LoggedInUserDTO>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<LoginQueryHandler> _logger;
    private readonly ITokenManager _authManager;

    public LoginQueryHandler(IUserRepository userRepository, ILogger<LoginQueryHandler> logger, ITokenManager authManager)
    {
        _userRepository = userRepository;
        _logger = logger;
        _authManager = authManager;
    }

    public async Task<Result<LoggedInUserDTO>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result<LoggedInUserDTO>.ArgumentNullResult();
        }
        try
        {            
            var user = await _userRepository.GetUserByEmailAsync(request.Email, cancellationToken);
            if (user is null || user.Deleted)
            {
                return Result<LoggedInUserDTO>.FailureResult("Invalid login attempt.");
            }
            var passwordVerificationResult = user.VerifyPassword(request.Password);
            if (passwordVerificationResult.IsFailure)
            {
                return Result<LoggedInUserDTO>.FailureResult("Invalid login attempt.");
            }
            var loggedInUser = LoggedInUserDTO.FromDomain(user);
            if (_authManager.GenerateTokensAndSetCookies(loggedInUser, request.RememberMe))
            {
                return Result<LoggedInUserDTO>.SuccessResult(loggedInUser);
            }            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while handling the Login process for the request: {request}.");
        }
        
        return Result<LoggedInUserDTO>.FailureResult("Invalid login attempt.");
    }
}
