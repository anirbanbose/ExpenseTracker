using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.UseCases.User.Commands;

public class RegistrationCommandHandler : IRequestHandler<RegistrationCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegistrationCommandHandler> _logger;
    public RegistrationCommandHandler(IUserRepository userRepository, ICurrencyRepository currencyRepository, IUnitOfWork unitOfWork, ILogger<RegistrationCommandHandler> logger)
    {
        _userRepository = userRepository;
        _currencyRepository = currencyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<Result> Handle(RegistrationCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result.ArgumentNullResult();
        }
        try
        {
            var currency = await _currencyRepository.GetCurrencyByIdAsync(request.PreferredCurrencyId, cancellationToken);
            if(currency is null)
            {
                _logger.LogWarning($"Preferred currency with Id - {request.PreferredCurrencyId} not found.");
                return Result.FailureResult("Account.UserRegistration");
            }
            var userResult = Domain.Models.User.Create(_userRepository, request.Email, request.Password, currency, request.FirstName, request.LastName, request.MiddleName);
            if (userResult.IsFailure)
            {
                _logger.LogWarning($"User registration failed for this request - {request}. Reason - {userResult.ErrorMessage}");
                return Result.FailureResult("Account.UserRegistration");
            }
            
            var user = userResult.Value;
            await _userRepository.AddUserAsync(user);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.SuccessResult();       
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error occurred while registering a user with request - {request}.");
        }
        return Result.FailureResult("Account.UserRegistration");
    }
}
