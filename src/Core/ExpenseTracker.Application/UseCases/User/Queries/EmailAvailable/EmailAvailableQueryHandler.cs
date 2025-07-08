using ExpenseTracker.Domain.Persistence.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.User.Queries;

public class EmailAvailableQueryHandler : IRequestHandler<EmailAvailableQuery, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<EmailAvailableQueryHandler> _logger;
    public EmailAvailableQueryHandler(IUserRepository userRepository, ILogger<EmailAvailableQueryHandler> logger) 
    {
        _userRepository = userRepository;
        _logger = logger;
    }
    public async Task<bool> Handle(EmailAvailableQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Email))
            {
                return false;
            }
            var user = await _userRepository.GetUserByEmailAsync(request.Email, cancellationToken);

            if (user is null)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while processing the email available request - {request}.");
        }            
        return false;
    }
}
