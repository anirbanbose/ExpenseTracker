
namespace ExpenseTracker.Domain.Models;

public record ExpenseCategoryId(Guid Value)
{
    public static implicit operator Guid(ExpenseCategoryId id) => id.Value;
    public static explicit operator ExpenseCategoryId(Guid value) => new ExpenseCategoryId(value);
    public static ExpenseCategoryId Create() => new ExpenseCategoryId(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
