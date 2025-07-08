

using ExpenseTracker.Domain.Models;
using FluentAssertions;

namespace ExpenseTracker.Domain.Tests.Models;

public class CurrencyTests
{
    [Fact]
    public void Create_Should_Fail_When_Code_Is_Empty()
    {
        //Act
        var result = Currency.Create(string.Empty, "United States Dollar", "$");

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().NotBeNull();
        result.Value.Should().BeNull();
        result.Code.Should().Be("DomainError.Currency.NullArgumentError");
    }

    [Fact]
    public void Create_Should_Fail_When_Name_Is_Empty()
    {
        //Act
        var result = Currency.Create("USD", string.Empty, "$");

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNull();
        result.Value.Should().BeNull();
        result.Code.Should().Be("DomainError.Currency.NullArgumentError");
    }

    [Fact]
    public void Create_Should_Success_When_All_Data_Is_Provided()
    {
        //Act
        var result = Currency.Create("USD", "United States Dollar", "$");

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Code.Should().Be("USD");
        result.Value.Name.Should().Be("United States Dollar");
        result.Value.Symbol.Should().Be("$");
    }
}
