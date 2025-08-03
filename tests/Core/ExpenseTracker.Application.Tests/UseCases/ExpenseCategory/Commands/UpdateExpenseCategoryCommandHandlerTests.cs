using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.UseCases.ExpenseCategory.Commands;
using ExpenseTracker.Domain.Persistence;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Domain.SharedKernel.Results;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ExpenseTracker.Application.Tests.UseCases.ExpenseCategory.Commands;

public class UpdateExpenseCategoryCommandHandlerTests
{
    private readonly Mock<IExpenseCategoryRepository> _expenseCategoryRepositoryMock = new();
    private readonly Mock<IAuthenticatedUserProvider> _authProviderMock = new();
    private readonly Mock<ILogger<UpdateExpenseCategoryCommandHandler>> _loggerMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();

    [Fact]
    public async Task Handle_Should_Update_The_Expense_Category()
    {
        var fakeUser = new FakeUserBuilder().WithEmail("test@test.com").Build();
        var existingCategory = new Domain.Models.ExpenseCategory("Test Expense Category", false, fakeUser.Id);
        var categoryId = existingCategory.Id;

        _authProviderMock
            .Setup(p => p.GetAuthenticatedUserAsync<Result>(It.IsAny<IUserNotAuthenticatedResultFactory<Result>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((fakeUser, null as Result));

        _expenseCategoryRepositoryMock
            .Setup(repo => repo.GetExpenseCategoryByIdAsync(categoryId, fakeUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _uowMock
            .Setup(uow => uow.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new UpdateExpenseCategoryCommandHandler(
            _authProviderMock.Object,
            _expenseCategoryRepositoryMock.Object,
            _uowMock.Object,
            _loggerMock.Object
        );

        var command = new UpdateExpenseCategoryCommand() { Id = categoryId, Name = "Updated Expense Category" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existingCategory.Name.Should().Be("Updated Expense Category");

        _expenseCategoryRepositoryMock.Verify(repo => repo.UpdateExpenseCategory(existingCategory), Times.Once);
        _uowMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
