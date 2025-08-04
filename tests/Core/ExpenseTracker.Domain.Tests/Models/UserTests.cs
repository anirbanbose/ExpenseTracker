using ExpenseTracker.Domain.Models;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.Utils;
using FluentAssertions;
using Moq;

namespace ExpenseTracker.Domain.Tests.Models;

public class UserTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public UserTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
    }

    [Fact]
    public void Create_Should_Fail_When_Email_Is_Empty()
    {
        // Arrange
        _userRepositoryMock
            .Setup(r => r.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null); 

        var currency = new Currency("USD", "United States Dollar", "$");

        // Act
        var result = User.Create(
            _userRepositoryMock.Object,
            string.Empty,  // Empty email
            "Password123!",
            currency,
            "John",
            "Doe");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNull();
        result.Value.Should().BeNull();
        result.ErrorCode.Should().Be(Constants.ValidationErrorCode);
    }

    [Fact]
    public void Create_Should_Return_Success_When_Valid_Data_And_Email_Available()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        var currency = new Currency("USD", "United States Dollar", "$");

        // Act
        var result = User.Create(_userRepositoryMock.Object, "john@example.com", "Password123!", currency, "John", "Doe");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be("john@example.com");
    }

    [Fact]
    public void VerifyPassword_Should_Fail_When_Wrong_Password_Provided()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((User?)null);
        var currency = new Currency("USD", "United States Dollar", "$");

        // Act
        var userResult = User.Create(_userRepositoryMock.Object, "john@example.com", "Password123!", currency, "John", "Doe");
        var user = userResult.Value;
        var result = user.VerifyPassword("WrongPassword");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("User.InvalidPassword");
        result.ErrorMessage.Should().NotBeNull();
    }

    [Fact]
    public void ChangePassword_Should_Fail_When_Empty_String_Is_Provided()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((User?)null);
        var currency = new Currency("USD", "United States Dollar", "$");

        // Act
        var userResult = User.Create(_userRepositoryMock.Object, "john@example.com", "Password123!", currency, "John", "Doe");
        var user = userResult.Value;
        var result = user.ChangePassword(string.Empty);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(Constants.ValidationErrorCode);
        result.ErrorMessage.Should().NotBeNull();
    }

    [Fact]
    public void ChangePassword_Should_Set_A_New_Password()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((User?)null);
        var currency = new Currency("USD", "United States Dollar", "$");

        // Act
        var userResult = User.Create(_userRepositoryMock.Object, "john@example.com", "Password123!", currency, "John", "Doe");
        var user = userResult.Value;
        var result = user.ChangePassword("NewPassword");

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void UpdateName_Should_Fail_When_Empty_String_Is_Provided_For_First_Name()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((User?)null);
        var currency = new Currency("USD", "United States Dollar", "$");

        // Act
        var userResult = User.Create(_userRepositoryMock.Object, "john@example.com", "Password123!", currency, "John", "Doe");
        var user = userResult.Value;
        var result = user.UpdateName(string.Empty, "Doe", null);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(Constants.ValidationErrorCode);
        result.ErrorMessage.Should().NotBeNull();
    }

    [Fact]
    public void UpdateName_Should_Fail_When_Empty_String_Is_Provided_For_Last_Name()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((User?)null);
        var currency = new Currency("USD", "United States Dollar", "$");

        // Act
        var userResult = User.Create(_userRepositoryMock.Object, "john@example.com", "Password123!", currency, "John", "Doe");
        var user = userResult.Value;
        var result = user.UpdateName("John", string.Empty, null);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(Constants.ValidationErrorCode);
        result.ErrorMessage.Should().NotBeNull();
    }

    [Fact]
    public void UpdateName_Should_Update_FirstName_LastName_MiddleName()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((User?)null);
        var currency = new Currency("USD", "United States Dollar", "$");

        // Act
        var userResult = User.Create(_userRepositoryMock.Object, "john@example.com", "Password123!", currency, "John", "Doe");
        var user = userResult.Value;
        var result = user.UpdateName("First", "Last", "Middle");

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Name.FirstName.Should().Be("First");
        user.Name.LastName.Should().Be("Last");
        user.Name.MiddleName.Should().Be("Middle");
    }

    [Fact]
    public void AddExpenseCategory_Should_Add_An_ExpenseCategory_To_The_User()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((User?)null);
        var currency = new Currency("USD", "United States Dollar", "$");


        // Act
        var userResult = User.Create(_userRepositoryMock.Object, "john@example.com", "Password123!", currency, "John", "Doe");
        var user = userResult.Value;
        user.AddExpenseCategory("Test Category");

        // Assert
        user.ExpenseCategories.Count().Should().Be(1);
    }
}
