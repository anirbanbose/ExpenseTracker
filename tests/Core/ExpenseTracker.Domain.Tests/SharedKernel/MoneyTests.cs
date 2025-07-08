using ExpenseTracker.Domain.SharedKernel;
using FluentAssertions;

namespace ExpenseTracker.Domain.Tests.SharedKernel;

public class MoneyTests
{
    [Fact]
    public void Money_Equality_Works()
    {
        //Arrange
        var money1 = new Money(100, "USD", "$");
        var money2 = new Money(100, "USD", "$");

        //Act
        var isEqual1 = money1.Equals(money2);
        var isEqual2 = money2.Equals(money1);
        var isEqual3 = money1 == money2;

        //Assert
        isEqual1.Should().BeTrue();
        isEqual2.Should().BeTrue();
        isEqual3.Should().BeTrue();
    }

    [Fact]
    public void Short_Formatted_Amount_Shows_Currency_Symbol_And_Amount()
    {
        //Arrange
        var money = new Money(100, "USD", "$");

        // Assert
        money.ShortFormattedAmount.Should().Be("$100.00");
    }

    [Fact]
    public void Short_Formatted_Amount_Shows_Currency_Code_And_Amount_If_Symbol_Is_Not_Present()
    {
        //Arrange
        var money = new Money(100, "USD", null);

        // Assert
        money.ShortFormattedAmount.Should().Be("USD100.00");
    }

    [Fact]
    public void Long_Formatted_Amount_Shows_Currency_Code_Symbol_And_Amount()
    {
        //Arrange
        var money = new Money(100, "USD", "$");

        // Assert
        money.LongFormattedAmount.Should().Be("USD $100.00");
    }

    [Fact]
    public void Add_Should_Create_New_Money_Object_With_Added_Amounts_If_Currency_Codes_Are_Same()
    {
        //Arrange
        var money1 = new Money(100, "USD", "$");
        var money2 = new Money(200, "USD", "$");

        //Act
        var addedAmount = money1.Add(money2);

        //Assert
        addedAmount.Should().Be(new Money(300, "USD", "$"));
        addedAmount.ShortFormattedAmount.Should().Be("$300.00");
    }

    [Fact]
    public void Subtract_Should_Create_New_Money_Object_With_Subtracted_Amounts_If_Currency_Codes_Are_Same()
    {
        //Arrange
        var money1 = new Money(100, "USD", "$");
        var money2 = new Money(200, "USD", "$");

        //Act
        var subtractedAmount = money2.Subtract(money1);

        //Assert
        subtractedAmount.Should().Be(new Money(100, "USD", "$"));
        subtractedAmount.ShortFormattedAmount.Should().Be("$100.00");
    }

    [Fact]
    public void Add_And_Subtract_Should_Fail_If_Currency_Codes_Are_Not_Same()
    {
        //Arrange
        var money1 = new Money(100, "USD", "$");
        var money2 = new Money(200, "GBP", null);

        //Act
        Action action1 = () => { var addedAmount = money1.Add(money2); };
        Action action2 = () => { var subtractedAmount = money2.Subtract(money1); };

        //Assert
        action1.Should().Throw<InvalidOperationException>().WithMessage("Cannot add two Money objects with different currencies.");
        action2.Should().Throw<InvalidOperationException>().WithMessage("Cannot subtract two Money objects with different currencies.");
    }
}
