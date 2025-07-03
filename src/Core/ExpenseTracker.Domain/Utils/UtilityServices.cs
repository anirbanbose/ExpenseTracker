namespace ExpenseTracker.Domain.Utils;

public class UtilityServices
{
    public static bool ByteArraysEqual(byte[] a, byte[] b)
    {
        if (a == null && b == null)
        {
            return true;
        }
        if (a == null || b == null || a.Length != b.Length)
        {
            return false;
        }
        var areSame = true;
        for (var i = 0; i < a.Length; i++)
        {
            areSame &= (a[i] == b[i]);
        }
        return areSame;
    }

    public static string GetMonthName(int? monthNumber)
    {
        if (!monthNumber.HasValue || monthNumber < 1 || monthNumber > 12)
        {
            throw new Exception("Invalid Month Number");
        }

        DateTime date = new DateTime(2024, monthNumber.Value, 1); // Year doesn't matter here
        return date.ToString("MMMM");
    }
}
