using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ExpenseTracker.Infrastructure.Persistence.Tests.Repositories;

public class ExpenseRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ExpenseRepository _repository;

    public ExpenseRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"ExpenseDb_{Guid.NewGuid()}")
            .Options;

        var mockPublisher = new Mock<IPublisher>();
        _context = new ApplicationDbContext(options, mockPublisher.Object);

        _repository = new ExpenseRepository(_context);

        // Seed basic data
        var user = new FakeUserBuilder().WithDefaults().Build();
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void Dispose() => _context?.Dispose();

    [Fact]
    public async Task AddExpenseAsync_Should_Add_Expense()
    {
        // Arrange
        var user = _context.Users.First();
        var expense = new FakeExpenseBuilder()
            .WithAllDetails("Lunch", "Restaurant", DateTime.UtcNow, user.Id, 200, "USD", "$")
            .Build();

        // Act
        await _repository.AddExpenseAsync(expense);
        await _context.SaveChangesAsync(CancellationToken.None);

        // Assert
        var result = await _context.Expenses.FirstOrDefaultAsync();
        result.Should().NotBeNull();
        result!.Description.Should().Be("Lunch");
    }

    [Fact]
    public async Task GetRecentExpensesAsync_Should_Return_Expenses()
    {
        // Arrange
        var user = _context.Users.First();
        var builder = new FakeExpenseBuilder();
        decimal randomAmount = 0;
        Random rnd = new Random();
        for (int i = 0; i < 10; i++)
        {
            randomAmount = rnd.Next(10, 1000);
            _context.Expenses.Add(builder.WithAmountAndDescription($"Expense {i}", user.Id, randomAmount).Build());
        }
        await _context.SaveChangesAsync(CancellationToken.None);

        // Act
        var results = await _repository.GetRecentExpensesAsync(user.Id, 3, CancellationToken.None);

        // Assert
        results.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetExpenseByIdAsync_Should_Return_Expense()
    {
        // Arrange
        var user = _context.Users.First();
        var expense = new FakeExpenseBuilder().WithUserId(user.Id).Build();
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetExpenseByIdAsync(expense.Id, user.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(expense.Id);
    }

    [Fact]
    public async Task GetExpensesByUserIdAsync_Should_Return_Expenses_For_User()
    {
        // Arrange
        var user = _context.Users.First();
        var builder = new FakeExpenseBuilder();
        _context.Expenses.Add(builder.WithAmountAndDescription("Test 1", user.Id, 100).Build());
        _context.Expenses.Add(builder.WithAmountAndDescription("Test 2", user.Id, 300).Build());
        await _context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetExpensesByUserIdAsync(user.Id, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateExpense_Should_Mark_As_Modified()
    {
        // Arrange
        var user = _context.Users.First();
        var expense = new FakeExpenseBuilder().WithUserId(user.Id).Build();
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync(CancellationToken.None);

        // Act
        expense.UpdateDescription("Updated Description");
        _repository.UpdateExpense(expense);
        await _context.SaveChangesAsync(CancellationToken.None);

        // Assert
        var result = await _context.Expenses.FirstOrDefaultAsync();
        result!.Description.Should().Be("Updated Description");
    }


    [Fact]
    public async Task SearchExpensesAsync_Should_Return_Expenses_Should_Search_Expenses()
    {
        // Arrange
        var user = _context.Users.First();
        var builder = new FakeExpenseBuilder();
        _context.Expenses.Add(builder.WithAmountAndDescription("Test 1", user.Id, 100).Build());
        _context.Expenses.Add(builder.WithAmountAndDescription("Test 2", user.Id, 300).Build());
        
        await _context.SaveChangesAsync(CancellationToken.None);        
        // Act
        var results = await _repository.SearchExpensesAsync(
            null, 
            null, 
            DateTime.UtcNow.Date, 
            null, 
            user.Id, 
            0,   // page index
            10,  // page size
            ExpenseListOrder.ExpenseDate, // sort by
            true, // ascending
            CancellationToken.None);

        // Assert
        results.TotalCount.Should().Be(2);
        results.Items.Should().HaveCount(2);
    }
}
