using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Application.Validation;

public class NotEmptyGuidAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is Guid guid)
        {
            return guid != Guid.Empty;
        }
        return false;
    }
}



