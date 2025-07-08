using ExpenseTracker.Domain.Models;
using ExpenseTracker.Domain.SharedKernel;
using FluentAssertions;

namespace ExpenseTracker.Domain.Tests.Models;

public class ExpenseTests
{
    [Fact]
    public void Create_Should_Be_Successful_When_All_Data_IsProvided()
    {
        // Arrange
        Money amount = new Money(100, "USD", "$");
        ExpenseCategoryId expenseCategoryId = new ExpenseCategoryId(Guid.NewGuid());
        UserId userId = new UserId(Guid.NewGuid());

        //Act
        var expense = new Expense(amount, "Test", expenseCategoryId, DateTime.Now, userId);

        // Assert
        expense.Should().NotBeNull();
    }

    [Fact]
    public void Update_Should_Be_Modify_All_Data_Values()
    {
        // Arrange        
        var expense = new Expense(new Money(100, "USD", "$"), "Old", new ExpenseCategoryId(Guid.NewGuid()), DateTime.Now.AddDays(-1), new UserId(Guid.NewGuid()));

        Money amountToBeUpdated = new Money(200, "USD", "$");
        ExpenseCategoryId expenseCategoryIdToBeUpdated = new ExpenseCategoryId(Guid.NewGuid());
        DateTime expenseDateToBeUpated = DateTime.Now;

        //Act
        expense.Update(amountToBeUpdated, "New", expenseCategoryIdToBeUpdated, expenseDateToBeUpated);

        // Assert
        expense.Description.Should().Be("New");
        expense.ExpenseAmount.Amount.Should().Be(200);
        expense.CategoryId.Should().Be(expenseCategoryIdToBeUpdated);
        expense.ExpenseDate.Should().Be(expenseDateToBeUpated);
    }
}
