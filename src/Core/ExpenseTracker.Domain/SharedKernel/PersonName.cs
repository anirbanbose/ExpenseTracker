namespace ExpenseTracker.Domain.SharedKernel;

public record PersonName(string FirstName, string LastName, string? MiddleName) 
{
    public string FullName => $"{FirstName} {(!string.IsNullOrEmpty(MiddleName) ? MiddleName + " " : string.Empty)}{LastName}";

    public override string ToString()
    {
        return FullName;
    }
}