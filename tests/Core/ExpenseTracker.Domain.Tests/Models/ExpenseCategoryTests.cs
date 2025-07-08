

using ExpenseTracker.Domain.Models;
using FluentAssertions;

namespace ExpenseTracker.Domain.Tests.Models;

public class ExpenseCategoryTests
{
    [Fact]
    public void Create_Should_Be_Successful_When_All_Data_IsProvided()
    {
        // Arrange
        UserId userId = new UserId(Guid.NewGuid());

        //Act
        var expenseCategory = new ExpenseCategory("Test Category", true, userId);

        // Assert
        expenseCategory.Should().NotBeNull();
    }

    [Fact]
    public void Update_Should_Be_Modify_Category_Name()
    {
        // Arrange
        UserId userId = new UserId(Guid.NewGuid());
        var expenseCategory = new ExpenseCategory("Test Category", true, userId);
        string newCategoryName = "New Category";

        //Act
        expenseCategory.UpdateName(newCategoryName);

        // Assert
        expenseCategory.Name.Should().Be("New Category");
    }
}
