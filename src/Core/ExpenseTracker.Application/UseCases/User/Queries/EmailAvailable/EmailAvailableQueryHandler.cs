using ExpenseTracker.Domain.Persistence.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.User.Queries;

public class EmailAvailableQueryHandler : BaseHandler, IRequestHandler<EmailAvailableQuery, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<EmailAvailableQueryHandler> _logger;
    public EmailAvailableQueryHandler(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, ILogger<EmailAvailableQueryHandler> logger) : base(httpContextAccessor)
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
            _logger?.LogError(ex.Message, ex);
        }
            
        return false;
    }
}
