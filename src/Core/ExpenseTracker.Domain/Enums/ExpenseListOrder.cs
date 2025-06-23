using System.Linq.Expressions;

namespace ExpenseTracker.Domain.Enums;


public enum ExpenseListOrder
{
    ExpenseAmount = 1,
    ExpenseDate,
    ExpenseCategory,
}


public static class ExpenseListOrderExtension
{
    public static Expression<Func<Models.Expense, object?>> ToOrderExpression(this ExpenseListOrder order) => order switch
    {
        ExpenseListOrder.ExpenseAmount => d => d.ExpenseAmount.Amount,
        ExpenseListOrder.ExpenseDate => d => d.ExpenseDate,
        ExpenseListOrder.ExpenseCategory => d => d.Category.Name,
        _ => d => d.ExpenseDate,
    };
}
