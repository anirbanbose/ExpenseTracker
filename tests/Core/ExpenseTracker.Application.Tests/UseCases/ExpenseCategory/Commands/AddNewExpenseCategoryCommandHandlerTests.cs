
using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Application.DTO.Dashboard;
using ExpenseTracker.Application.UseCases.ExpenseCategory.Commands;
using ExpenseTracker.Domain.Persistence.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ExpenseTracker.Application.Tests.UseCases.ExpenseCategory.Commands;

//internal class AddNewExpenseCategoryCommandHandlerTests
//{
//    private readonly Mock<IExpenseCategoryRepository> _expenseCategoryRepositoryMock = new();
//    private readonly Mock<ICurrentUserManager> _currentUserManagerMock = new();
//    private readonly Mock<IAuthenticatedUserProvider> _authProviderMock = new();
//    private readonly Mock<ILogger<AddNewExpenseCategoryCommandHandler>> _loggerMock = new();

//    [Fact]
//    public async Task Add_Should_Add_A_New_Expense_Category()
//    {
//        // Arrange
//        var fakeUser = new FakeUserBuilder().WithEmail("test@test.com").Build();
//        var expenseCategory = new Domain.Models.ExpenseCategory("Test Expense Category", false, fakeUser.Id);

//        _expenseCategoryRepositoryMock
//            .Setup(repo => repo.AddExpenseCategoryAsync(expenseCategory));

//        _authProviderMock
//        .Setup(p => p.GetAuthenticatedUserAsync<Guid?>(It.IsAny<IUserNotAuthenticatedResultFactory<Guid?>>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync((fakeUser, null))
//            .Verifiable();
//    }
//}
