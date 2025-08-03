using ExpenseTracker.Domain.DomainEvents;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.SharedKernel.Results;
using ExpenseTracker.Domain.Utils;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
namespace ExpenseTracker.Domain.Models;

public partial class User : Entity<UserId>
{
    private User(UserId id) : base(id) { }
    public static DomainResult<User> Create(IUserRepository userRepository, string email, string password, Currency preferredCurrency, string firstName, string lastName, string? middleName = default)
    {
        var validattionMsg = CheckFields(email, password, firstName, lastName);
        if (!string.IsNullOrEmpty(validattionMsg))
        {
            return DomainResult<User>.DomainValidationFailureResult(Constants.ValidationErrorCode, validattionMsg);
        }
        email = email.ToLower();
        Task<bool> task = Task.Run(async () => await CheckEmailAvailability(email, userRepository));
        var emailAvailable = task.Result;

        if (!emailAvailable)
        {
            return DomainResult<User>.DomainValidationFailureResult(Constants.ValidationErrorCode, "Email already registered.");
        }
        var user = new User(UserId.Create())
        {
            Email = email,
            Name = new PersonName(firstName, lastName, middleName),
        };
        user.SetPassword(password);

        user.Preference = UserPreference.Create(user.Id, preferredCurrency, false, false);

        user.AddDomainEvent(new UserRegisteredDomainEvent(user.Email, user.Name.FullName));

        return DomainResult<User>.SuccessResult(user);
    }

    public void AddExpenseCategory(string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName)) return;
        _expenseCategories.Add(new ExpenseCategory(categoryName, false, Id));
    }
    private DomainResult SetPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password)) return DomainResult.DomainValidationFailureResult(Constants.ValidationErrorCode, "Password is null");
        byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
        string hashed = Convert.ToBase64String(GetHashed(password, salt));

        PasswordHash = hashed;
        PasswordSalt = Convert.ToBase64String(salt);
        return DomainResult.SuccessResult();
    }

    public DomainResult ChangePassword(string password)
    {
        var setPasswordResult = SetPassword(password);
        AddDomainEvent(new PasswordChangedDomainEvent(Email, Name.FullName));
        return setPasswordResult;
    }

    public DomainResult UpdateName(string firstName, string lastName, string? middleName)
    {        
        if (string.IsNullOrWhiteSpace(firstName)) return DomainResult.DomainValidationFailureResult(Constants.ValidationErrorCode, "First Name is null"); 
        if (string.IsNullOrWhiteSpace(lastName)) return DomainResult.DomainValidationFailureResult(Constants.ValidationErrorCode, "Last Name is null");
        string oldName = Name.FullName;
        Name = new PersonName(firstName, lastName, middleName);
        AddDomainEvent(new ProfileUpdatedDomainEvent(Email, oldName, Name.FullName));
        return DomainResult.SuccessResult();
    }

    public DomainResult VerifyPassword(string passwordToBeVerified)
    {
        if (string.IsNullOrWhiteSpace(passwordToBeVerified)) return DomainResult.DomainValidationFailureResult(Constants.ValidationErrorCode, "Password is null");
        byte[] salt = Convert.FromBase64String(PasswordSalt);
        byte[] hashedValueOfTheProvidedPassword = GetHashed(passwordToBeVerified, salt);

        byte[] hashedValueOfThePassword = Convert.FromBase64String(PasswordHash);

        if (!UtilityServices.ByteArraysEqual(hashedValueOfThePassword, hashedValueOfTheProvidedPassword))
        {
            return DomainResult.DomainValidationFailureResult("User.InvalidPassword", "Password is invalid");
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

    private static string CheckFields(string email, string password, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(email)) return "Email is null";
        if (string.IsNullOrWhiteSpace(password)) return "Password is null"; 
        if (string.IsNullOrWhiteSpace(firstName)) return "First Name is null"; 
        if (string.IsNullOrWhiteSpace(lastName)) return "Last Name is null"; 
        return string.Empty;
    }
}