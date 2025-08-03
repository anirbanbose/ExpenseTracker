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

public class RecentExpensesQueryHandlerTests 
{
    private readonly Mock<IExpenseRepository> _expenseRepositoryMock = new();
    private readonly Mock<IAuthenticatedUserProvider> _authProviderMock = new();
    private readonly Mock<ILogger<RecentExpensesQueryHandler>> _loggerMock = new();


    [Fact]
    public async Task Handle_Should_Return_Recent_Expenses()
    {
        // Arrange
        var fakeUser = new FakeUserBuilder().WithEmail("test@test.com").Build();
        var recentExpenses = new List<Expense>
        {
            new FakeExpenseBuilder().WithDefaults().Build(),
            //new FakeExpenseBuilder().WithDetails("Recent Groceries", "Groceries", DateTime.UtcNow, fakeUser.Id, 100, "USD", "$").Build(),
            //new FakeExpenseBuilder().WithDetails("Rent", "Others", DateTime.UtcNow.AddDays(-1), fakeUser.Id, 1000, "USD", "$").Build(),
        };

        _expenseRepositoryMock
            .Setup(repo => repo.GetRecentExpensesAsync(fakeUser.Id, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recentExpenses);


        _authProviderMock.Setup(p => p.GetAuthenticatedUserAsync<Result<List<RecentExpenseListDTO>>>(It.IsAny<IUserNotAuthenticatedResultFactory<Result<List<RecentExpenseListDTO>>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((fakeUser, null));

        var handler = new RecentExpensesQueryHandler(
            _authProviderMock.Object,
            _expenseRepositoryMock.Object,
            _loggerMock.Object
        );

        var request = new RecentExpensesQuery(5);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_Should_Return_Authentication_Error_If_User_Does_Not_Exist()
    {
        // Arrange
        User? nullUser = null;
        var recentExpenses = new List<Expense>();

        _expenseRepositoryMock
            .Setup(repo => repo.GetRecentExpensesAsync(new UserId(Guid.NewGuid()), 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recentExpenses);

        _authProviderMock
        .Setup(p => p.GetAuthenticatedUserAsync<Result<List<RecentExpenseListDTO>>>(It.IsAny<IUserNotAuthenticatedResultFactory<Result<List<RecentExpenseListDTO>>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((nullUser, Result<List<RecentExpenseListDTO>>.UserNotAuthenticatedResult()));

        var handler = new RecentExpensesQueryHandler(
            _authProviderMock.Object,
            _expenseRepositoryMock.Object,
            _loggerMock.Object
        );

        var request = new RecentExpensesQuery(5);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }
}
