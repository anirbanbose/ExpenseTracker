using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.Dashboard;
using ExpenseTracker.Application.UseCases.Dashboard.Queries;
using ExpenseTracker.Domain.Models;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.SharedKernel.Results;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ExpenseTracker.Application.Tests.UseCases.Dashboard.Queries;

public class ExpenseSummaryQueryHandlerTests
{
    private readonly Mock<IExpenseRepository> _expenseRepositoryMock = new();
    private readonly Mock<IAuthenticatedUserProvider> _authProviderMock = new();
    private readonly Mock<ILogger<ExpenseSummaryQueryHandler>> _loggerMock = new();

    [Fact]
    public async Task Handle_Should_Return_Success_For_ExpenseSummary()
    {
        // Arrange
        var fakeUser = new FakeUserBuilder().WithEmail("test@test.com").Build();
        var expenses = new List<Expense>
        {
            new FakeExpenseBuilder().WithDetails("Recent Groceries", "Groceries", DateTime.UtcNow, fakeUser.Id, 100, "USD", "$").Build(),
            new FakeExpenseBuilder().WithDetails("Rent", "Others", DateTime.UtcNow, fakeUser.Id, 1000, "USD", "$").Build(),
        };

        _expenseRepositoryMock
            .Setup(repo => repo.GetExpensesByUserIdAsync(fakeUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expenses);

        _authProviderMock.Setup(p => p.GetAuthenticatedUserAsync<Result<ExpenseSummaryDTO>>(It.IsAny<IUserNotAuthenticatedResultFactory<Result<ExpenseSummaryDTO>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((fakeUser, null));


        var handler = new ExpenseSummaryQueryHandler(
            _authProviderMock.Object,
            _expenseRepositoryMock.Object,
            _loggerMock.Object
        );

        var request = new ExpenseSummaryQuery();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalExpenses.Should().HaveCount(1);
        result.Value.TotalExpenses[0].Should().Be("$1,100.00");
        result.Value.CurrentMonthExpenses.Should().HaveCount(1);
        result.Value.CurrentMonthExpenses[0].Should().Be("$1,100.00");
        result.Value.CurrentMonthTopCategoryExpense.Should().HaveCount(1);
        result.Value.CurrentMonthTopCategoryExpense[0].CategoryName.Should().Be("Others");
        result.Value.CurrentMonthTopCategoryExpense[0].Expense.Should().Be("$1,000.00");
    }
}
