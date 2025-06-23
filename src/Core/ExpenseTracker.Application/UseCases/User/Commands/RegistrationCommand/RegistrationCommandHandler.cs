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
                return Result.FailureResult("Account.UserRegistration", "Preferred currency not found.");
            }
            var userResult = Domain.Models.User.Create(_userRepository, request.Email, request.Password, currency, request.FirstName, request.LastName, request.MiddleName);
            if (userResult.IsFailure)
            {
                return Result.FailureResult("Account.UserRegistration", userResult.ErrorMessage);
            }
            
            var user = userResult.Value;
            await _userRepository.AddUserAsync(user);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.SuccessResult();       
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex.Message, ex);
        }
        return Result.FailureResult("Account.UserRegistration", "Account registration failed.");
    }
}
