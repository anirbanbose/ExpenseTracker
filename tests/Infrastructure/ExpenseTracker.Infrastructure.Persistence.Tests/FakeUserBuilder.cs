using ExpenseTracker.Domain.Models;
using ExpenseTracker.Domain.Persistence.Repositories;
using Moq;

namespace ExpenseTracker.Infrastructure.Persistence.Tests;

internal class FakeUserBuilder
{
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public FakeUserBuilder()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
    }

    private string _email = "test@example.com";
    private string _password = "password";
    private string _firstName = "John";
    private string _lastName = "Doe";
    private Currency _currency = new Currency("USD", "US Dollar", "$");

    public FakeUserBuilder WithEmail(string email) { _email = email; return this; }

    public FakeUserBuilder WithDefaults()
    {
        return this;
    }


    public User Build()
    {
        _userRepositoryMock
            .Setup(r => r.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var userResult = User.Create(_userRepositoryMock.Object, _email, _password, _currency, _firstName, _lastName);

        return userResult.Value;
    }
}
