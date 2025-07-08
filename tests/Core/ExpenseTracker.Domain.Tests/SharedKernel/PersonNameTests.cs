using ExpenseTracker.Domain.SharedKernel;
using FluentAssertions;
namespace ExpenseTracker.Domain.Tests.SharedKernel;

public class PersonNameTests
{
    [Fact]
    public void PersonName_Equality_Works()
    {
        //Arrange
        var name1 = new PersonName("John", "Doe", "A");
        var name2 = new PersonName("John", "Doe", "A");

        //Act
        var isEqual1 = name1.Equals(name2);
        var isEqual2 = name2.Equals(name1);
        var isEqual3 = name1 == name2;

        //Assert
        isEqual1.Should().BeTrue();
        isEqual2.Should().BeTrue();
        isEqual3.Should().BeTrue();
    }

    [Fact]
    public void PersonName_ToString_Should_Show_Formatted_Name()
    {
        //Arrange
        var name1 = new PersonName("John", "Doe", "A");
        var name2 = new PersonName("John", "Doe");

        //Act
        var formattedName1 = name1.ToString();
        var formattedName2 = name2.ToString();


        //Assert
        formattedName1.Should().Be("John A Doe");
        formattedName2.Should().Be("John Doe");
    }
}
