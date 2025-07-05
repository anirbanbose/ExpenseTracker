using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.SharedKernel.Errors;
using ExpenseTracker.Domain.Utils;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
namespace ExpenseTracker.Domain.Models;

public partial class User : Entity<UserId>
{
    private User(UserId id) : base(id) { }
    public static DomainResult<User> Create(IUserRepository userRepository, string email, string password, Currency preferredCurrency, string firstName, string lastName, string? middleName = default)
    {
        var userFieldValidationError = CheckFields(email, password, firstName, lastName);
        if (userFieldValidationError.Type != ErrorType.NoError)
        {
            return DomainResult<User>.DomainValidationFailureResult(userFieldValidationError);
        }
        email = email.ToLower();
        Task<bool> task = Task.Run(async () => await CheckEmailAvailability(email, userRepository));
        var emailAvailable = task.Result;

        if (!emailAvailable)
        {
            return DomainResult<User>.DomainValidationFailureResult("DomainError.User.EmailNotAvailable", "Email already registered.");
        }
        var user = new User(UserId.Create())
        {
            Email = email,
            Name = new PersonName(firstName, lastName, middleName),
        };
        user.SetPassword(password);

        user.Preference = UserPreference.Create(user.Id, preferredCurrency, false, false);

        //user.AddDomainEvent(new UserRegisteredDomainEvent(user.UserName, user.Name.FullName));

        return DomainResult<User>.SuccessResult(user);
    }

    public void AddExpenseCategory(string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName)) return;
        _expenseCategories.Add(new ExpenseCategory(categoryName, false, Id));
    }
    private DomainResult SetPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password)) return DomainResult.DomainValidationFailureResult("DomainError.User.NullArgumentError", "Password is null");
        byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
        string hashed = Convert.ToBase64String(GetHashed(password, salt));

        PasswordHash = hashed;
        PasswordSalt = Convert.ToBase64String(salt);
        return DomainResult.SuccessResult();
    }

    public DomainResult ChangePassword(string password)
    {
        var setPasswordResult = SetPassword(password);
        //AddDomainEvent(new PasswordChangedDomainEvent(UserName, Name.FullName));
        return setPasswordResult;
    }

    public DomainResult UpdateName(string firstName, string lastName, string? middleName)
    {
        if (string.IsNullOrWhiteSpace(firstName)) return DomainResult.DomainValidationFailureResult("DomainError.User.NullArgumentError", "First Name is null"); 
        if (string.IsNullOrWhiteSpace(lastName)) return DomainResult.DomainValidationFailureResult("DomainError.User.NullArgumentError", "Last Name is null");
        Name = new PersonName(firstName, lastName, middleName);

        return DomainResult.SuccessResult();
    }

    public DomainResult VerifyPassword(string passwordToBeVerified)
    {
        if (string.IsNullOrWhiteSpace(passwordToBeVerified)) return DomainResult.DomainValidationFailureResult("DomainError.User.NullArgumentError", "Password is null");
        byte[] salt = Convert.FromBase64String(PasswordSalt);
        byte[] hashedValueOfTheProvidedPassword = GetHashed(passwordToBeVerified, salt);

        byte[] hashedValueOfThePassword = Convert.FromBase64String(PasswordHash);

        if (!UtilityServices.ByteArraysEqual(hashedValueOfThePassword, hashedValueOfTheProvidedPassword))
        {
            return DomainResult.DomainValidationFailureResult("DomainError.User.InvalidPassword", "Password is invalid");
        }
        else
        {
            return DomainResult.SuccessResult();
        }
    }

    private byte[] GetHashed(string password, byte[] salt)
    {
        return KeyDerivation.Pbkdf2(password: password!, salt: salt, prf: KeyDerivationPrf.HMACSHA256, iterationCount: 100000, numBytesRequested: 256 / 8);
    }

    private static async Task<bool> CheckEmailAvailability(string email, IUserRepository userRepository)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        var user = await userRepository.GetUserByEmailAsync(email, cts.Token);
        if (user is not null)
        {
            return false;
        }
        return true;
    }

    private static Error CheckFields(string email, string password, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(email)) return new Error("DomainError.User.NullArgumentError", "Email is null", ErrorType.Validation);
        if (string.IsNullOrWhiteSpace(password)) return new Error("DomainError.User.NullArgumentError", "Password is null", ErrorType.Validation);
        if (string.IsNullOrWhiteSpace(firstName)) return new Error("DomainError.User.NullArgumentError", "First Name is null", ErrorType.Validation);
        if (string.IsNullOrWhiteSpace(lastName)) return new Error("DomainError.User.NullArgumentError", "Last Name is null", ErrorType.Validation);
        return Error.None();
    }
}