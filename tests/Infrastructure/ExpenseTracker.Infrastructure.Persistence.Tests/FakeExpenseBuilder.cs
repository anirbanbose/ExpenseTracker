using ExpenseTracker.Domain.Models;
using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Infrastructure.Persistence.Tests;

internal class FakeExpenseBuilder
{
    private decimal _expenseAmount = 100;
    private string _currencyCode = "USD";
    private string? _currencySymbol = "$";
    private string _description = "Test Description";
    private string _categoryName = "Test Category";
    private DateTime _expenseDate = DateTime.UtcNow.Date;
    private UserId _userId = new UserId(Guid.NewGuid());

    public FakeExpenseBuilder WithDefaults()
    {
        return this;
    }

    public FakeExpenseBuilder WithUserId(UserId userId)
    {
        _userId = userId;
        return this;
    }

    public FakeExpenseBuilder WithAmountAndDescription(string description, UserId userId, decimal expenseAmount)
    {
        _expenseAmount = expenseAmount;
        _description = description;
        _userId = userId;
        return this;
    }

    public FakeExpenseBuilder WithAllDetails(string description, string categoryName, DateTime expenseDate, UserId userId, decimal expenseAmount, string currencyCode, string? currencySymbol = null)
    {
        _expenseAmount = expenseAmount;
        _currencyCode = currencyCode;
        _currencySymbol = currencySymbol;
        _description = description;
        _categoryName = categoryName;
        _expenseDate = expenseDate;
        _userId = userId;
        return this;
    }

    public Expense Build()
    {
        ExpenseCategory category = new ExpenseCategory(_categoryName, true);
        return new Expense(new Money(_expenseAmount, _currencyCode, _currencySymbol), _description, category, _expenseDate, _userId);
    }

}
