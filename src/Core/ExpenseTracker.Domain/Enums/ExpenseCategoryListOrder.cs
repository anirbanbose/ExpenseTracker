using System.Linq.Expressions;

namespace ExpenseTracker.Domain.Enums;

public enum ExpenseCategoryListOrder
{
    Name = 0,
    IsSystemCategory,
    TotalNumberOfExpenses,
}


public static class ExpenseCategoryListOrderExtension
{
    public static Expression<Func<Models.ExpenseCategory, object?>> ToOrderExpression(this ExpenseCategoryListOrder order) => order switch
    {
        ExpenseCategoryListOrder.Name => d => d.Name,
        ExpenseCategoryListOrder.IsSystemCategory => d => d.IsSystemCategory,
        ExpenseCategoryListOrder.TotalNumberOfExpenses => d => d.Expenses.Count,
        _ => d => d.Name,
    };
}
