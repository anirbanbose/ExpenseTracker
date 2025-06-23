using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

namespace ExpenseTracker.Domain.Utils;

public static class BasicTypeExtensions
{
    public static bool ContainsAny(this string theString, params string[] tokens)
    {
        if (theString == null)
        {
            return false;
        }

        string temp = theString.ToLower();
        return tokens.Any((string s) => temp.Contains(s.ToLower()));
    }

    public static bool ContainsAll(this string theString, params string[] tokens)
    {
        if (theString == null)
        {
            return false;
        }

        string text = theString.ToLower();
        foreach (string value in tokens)
        {
            if (!text.Contains(value))
            {
                return false;
            }
        }

        return true;
    }

    public static string ReplaceAny(this string theString, string replacement, params string[] tokens)
    {
        if (theString == null)
        {
            return null;
        }

        return tokens.Aggregate(theString, (string current, string t) => current.Replace(t, replacement));
    }

    public static bool EqualsAny(this string theString, params string[] args)
    {
        if (theString == null)
        {
            return false;
        }

        return args.Any((string token) => theString.Equals(token, StringComparison.CurrentCultureIgnoreCase));
    }

    public static bool IsNullOrWhitespace(this string theString)
    {
        return string.IsNullOrWhiteSpace(theString);
    }

    public static string[] RemoveEmpty(this IEnumerable<string> theStringArray)
    {
        return theStringArray?.Where((string s) => !s.IsNullOrWhitespace()).ToArray();
    }

    public static bool ToBool(this string theString)
    {
        if (theString == null)
        {
            throw new ArgumentNullException("theString");
        }

        if (theString.IsNullOrWhitespace())
        {
            return false;
        }

        theString = theString.ToLower();
        return theString.ToLower().EqualsAny("yes", "y", "1", "+", "true", "on");
    }

    public static int ToInt(this string theString, int defaultNumber = 0)
    {
        if (theString.IsNullOrWhitespace())
        {
            return defaultNumber;
        }

        int result;
        bool flag = int.TryParse(theString, out result);
        if (!flag)
        {
            if (theString.Contains("."))
            {
                theString = theString.SubstringTo(".");
            }

            flag = decimal.TryParse(theString, out var result2);
            if (flag)
            {
                result = (int)result2;
            }
        }

        if (!flag)
        {
            return defaultNumber;
        }

        return result;
    }

    public static int? ToNullableInt(this string theString, int nullableValue = 0)
    {
        int num = theString.ToInt(nullableValue);
        if (num == nullableValue)
        {
            return null;
        }

        return num;
    }

    public static bool IsDate(this string? theString)
    {
        if (string.IsNullOrEmpty(theString) || theString.IsNullOrWhitespace())
        {
            return false;
        }
        return DateTime.TryParse(theString, out _);
    }

    public static DateTime ToDate(this string theString, DateTime defaultDate = default(DateTime))
    {
        if (!DateTime.TryParse(theString, out var result))
        {
            return defaultDate;
        }

        return result;
    }

    public static DateTime? TryAsCsvDate(this string theString)
    {
        if (theString.IsNullOrWhitespace())
        {
            return null;
        }

        string[] array = theString.Split(',');
        if (array.Length != 3)
        {
            return null;
        }

        try
        {
            return new DateTime(array[0].ToInt(), array[1].ToInt(), array[2].ToInt());
        }
        catch
        {
            return null;
        }
    }

    public static string InsertInto(this string theString, string format)
    {
        return string.Format(format, theString);
    }

    public static string SubstringFrom(this string theString, string marker, bool shouldIncludeMarker = false)
    {
        if (theString == null)
        {
            return null;
        }

        int num = theString.IndexOf(marker, StringComparison.CurrentCultureIgnoreCase);
        if (num < 0)
        {
            return theString;
        }

        int startIndex = (shouldIncludeMarker ? num : (num + marker.Length));
        return theString.Substring(startIndex);
    }

    public static string SubstringTo(this string theString, string marker, bool shouldIncludeMarker = false)
    {
        if (theString == null)
        {
            return null;
        }

        int num = theString.IndexOf(marker, StringComparison.CurrentCultureIgnoreCase);
        if (num < 0)
        {
            return theString;
        }

        return theString[..(shouldIncludeMarker ? (num + marker.Length) : num)];
    }

    public static string SubstringBetween(this string theString, string marker1, string marker2)
    {
        return (theString?.SubstringFrom(marker1))?.SubstringTo(marker2);
    }

    public static string ToDefault(this string theString, string emptyText = "")
    {
        if (!theString.IsNullOrWhitespace())
        {
            return theString;
        }

        return emptyText;
    }

    public static bool IsValidEmail(this string theString, bool okayIfEmpty = false)
    {
        if (theString.IsNullOrWhitespace())
        {
            return okayIfEmpty;
        }

        try
        {
            return Regex.IsMatch(theString, "^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-z])@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-z][-\\w]*[0-9a-z]*\\.)+[a-z0-9][\\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250.0));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    public static DateTime? ToTime(this string time)
    {
        if (time.IsNullOrWhitespace())
        {
            return null;
        }

        string[] array = time.Split(':');
        if (array.Length != 2)
        {
            return null;
        }

        return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, array[0].ToInt(), array[1].ToInt(), 0);
    }

    public static IList<int> ToIntList(this string theString, char separator = ',')
    {
        List<int> list = new List<int>();
        if (theString.IsNullOrWhitespace())
        {
            return list;
        }

        string[] source = theString.Split(separator);
        list.AddRange(from t in source
                      select t.ToInt(-9999999) into number
                      where number != -9999999
                      select number);
        return list;
    }

    public static string Capitalize(this string theString)
    {
        if (string.IsNullOrWhiteSpace(theString))
        {
            return theString;
        }

        theString = theString.Trim().Replace("''", "'");
        if (theString.Length >= 3)
        {
            char c2 = theString[0];
            char c3 = theString[1];
            char c4 = theString[2];
            if (char.IsUpper(c2) && char.IsLower(c3) && char.IsUpper(c4))
            {
                return theString.Substring(0, 3) + theString.Substring(3).ToLower();
            }
        }

        if (theString.StartsWith("Mac") && theString.Length > 3)
        {
            char c5 = theString[3];
            string source = theString.Substring(4);
            if (char.IsUpper(c5) && source.All((char c) => char.IsLower(c)))
            {
                return theString;
            }
        }

        theString = theString.ToLower();
        string[] array = theString.Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < array.Length; i++)
        {
            string text = array[i];
            array[i] = text.Substring(0, 1).ToUpper() + text.Substring(1).ToLower();
            if (array[i].StartsWith("mc", StringComparison.OrdinalIgnoreCase))
            {
                string text2 = array[i].Substring(2);
                if (!text2.IsNullOrWhitespace() && text2.All((char c) => char.IsUpper(c) || char.IsLower(c)))
                {
                    array[i] = "Mc" + text2.Substring(0, 1).ToUpper() + text2.Substring(1).ToLower();
                }
            }
        }

        theString = string.Join(" ", array);
        string[] array2 = theString.Split('\'', StringSplitOptions.RemoveEmptyEntries);
        if (array2.Length > 1)
        {
            theString = $"{array2[0]}{39}{array2[1].Substring(0, 1).ToUpper() + array2[1].Substring(1).ToLower()}";
        }

        array2 = theString.Split('-', StringSplitOptions.RemoveEmptyEntries);
        if (array2.Length > 1)
        {
            for (int j = 0; j < array2.Length; j++)
            {
                string text3 = array2[j];
                array2[j] = text3.Substring(0, 1).ToUpper() + text3.Substring(1).ToLower();
            }

            theString = string.Join("-", array2);
        }

        return theString;
    }

    public static string TrimOrDefault(this string theString)
    {
        if (!theString.IsNullOrWhitespace())
        {
            return theString.Trim();
        }

        return null;
    }

    public static string TrimAll(this string text)
    {
        if (!text.IsNullOrWhitespace())
        {
            return Regex.Replace(text, "\\s\\s+", " ").Trim();
        }

        return text;
    }

    public static string ToFormat(this string theString, string format = "")
    {
        if (!format.IsNullOrWhitespace())
        {
            return string.Format(format, theString);
        }

        return theString;
    }

    public static IList<long> ToLongList(this string theString, char separator = ',')
    {
        List<long> list = new List<long>();
        if (theString.IsNullOrWhitespace())
        {
            return list;
        }

        string[] source = theString.Split(separator);
        list.AddRange(from t in source
                      select t.ToLong(-9999999L) into number
                      where number != -9999999
                      select number);
        return list;
    }

    public static long ToLong(this string theString, long defaultNumber = 0L)
    {
        if (theString.IsNullOrWhitespace())
        {
            return defaultNumber;
        }

        long result;
        bool flag = long.TryParse(theString, out result);
        if (!flag)
        {
            if (theString.Contains("."))
            {
                theString = theString.SubstringTo(".");
            }

            flag = decimal.TryParse(theString, out var result2);
            if (flag)
            {
                result = (long)result2;
            }
        }

        if (!flag)
        {
            return defaultNumber;
        }

        return result;
    }

    public static string SubstringMax(this string theString, int length, bool ellipsis = true)
    {
        if (theString.IsNullOrWhitespace())
        {
            return theString;
        }

        if (length > theString.Length)
        {
            return theString;
        }

        return theString[..Math.Min(theString.Length, length)] + (ellipsis ? "..." : "");
    }

    public static string ReplaceDiacritics(this string theString)
    {
        if (theString == null)
        {
            return null;
        }

        return new string((from c in theString.Normalize(NormalizationForm.FormD)
                           where char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark
                           select c).ToArray());
    }

    public static string ToBase64(this string theString)
    {
        if (theString.IsNullOrWhitespace())
        {
            return null;
        }

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(theString));
    }

    public static string FromBase64(this string base64EncodedData)
    {
        if (base64EncodedData.IsNullOrWhitespace())
        {
            return null;
        }

        byte[] bytes = Convert.FromBase64String(base64EncodedData);
        return Encoding.UTF8.GetString(bytes);
    }

    public static bool IsOn(this string theString)
    {
        return theString?.EqualsAny("on", "true", "ok") ?? false;
    }

    public static IList<string> ToStringList(this string theString, char sep = ',')
    {
        if (string.IsNullOrWhiteSpace(theString))
        {
            return new List<string>();
        }

        return (from t in theString.Split(sep)
                select t.Trim()).ToList();
    }

    public static Dictionary<string, string> ParseToDictionary(this string theString, char itemSep = '=', char listSep = ',')
    {
        if (theString.IsNullOrWhitespace())
        {
            return new Dictionary<string, string>();
        }

        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        string[] array = theString.Split(listSep);
        for (int i = 0; i < array.Length; i++)
        {
            string[] array2 = array[i].Split(itemSep);
            if (array2.Length == 2)
            {
                dictionary.Add(array2[0], array2[1]);
            }
        }

        return dictionary;
    }

    public static string PadRightText(this string theString, int maxLength, string pad)
    {
        if (theString.IsNullOrWhitespace())
        {
            return theString;
        }

        if (theString.Length >= maxLength)
        {
            return theString;
        }

        StringBuilder stringBuilder = new StringBuilder(theString);
        for (int i = theString.Length; i <= maxLength; i++)
        {
            stringBuilder.Append(pad);
        }

        return stringBuilder.ToString();
    }

    public static string Truncate(this string theString, int maxLength, bool applyEllipsis = true, string ellipsis = "...")
    {
        if (theString.IsNullOrWhitespace())
        {
            return theString;
        }

        string text = (applyEllipsis ? ellipsis : "");
        if (theString.Length <= maxLength)
        {
            return theString;
        }

        return theString.Substring(0, maxLength) + text;
    }

    public static string SubstringFromLast(this string theString, string occurrence)
    {
        if (occurrence.IsNullOrWhitespace() || theString.IsNullOrWhitespace())
        {
            return theString;
        }

        int num = theString.LastIndexOf(occurrence, StringComparison.Ordinal);
        return theString.Substring(num + occurrence.Length);
    }

    public static string Reverse(this string theString)
    {
        char[] array = theString.ToCharArray();
        Array.Reverse(array);
        return new string(array);
    }

    public static string Wrap(this string theString, string bracket, string empty = "")
    {
        if (string.IsNullOrEmpty(theString))
        {
            return empty;
        }

        return bracket switch
        {
            "(" => "(" + theString + ")",
            "[" => "[" + theString + "]",
            "<" => "<" + theString + ">",
            "{" => "{" + theString + "}",
            _ => bracket + theString + bracket,
        };
    }

    public static bool IsGuid(this string? value)
    {
        if(string.IsNullOrEmpty(value) || value.IsNullOrWhitespace())
        {
            return false;
        }
        return Guid.TryParse(value, out _);        
    }

    public static Guid? ToGuid(this string value)
    {
        return value.IsGuid()
            ? Guid.Parse(value)
            : null;
    }

    public static bool IsInteger(this string? value)
    {
        if (string.IsNullOrEmpty(value) || value.IsNullOrWhitespace())
        {
            return false;
        }
        return int.TryParse(value, out _);
    }

    public static bool IsNumber(this string? value)
    {
        if (string.IsNullOrEmpty(value) || value.IsNullOrWhitespace())
        {
            return false;
        }
        return decimal.TryParse(value, out _);
    }

    public static DateTime GetRandomDate(this Random random, DateTime minDate, DateTime maxDate)
    {
        int minYear = minDate.Year;
        int maxYear = maxDate.Year + 1;

        DateTime generatedDate = maxDate.AddDays(1).Date; 
        while (generatedDate < minDate || generatedDate > maxDate)
        {
            var year = random.Next(minYear, maxYear);
            var month = random.Next(1, 13);
            var noOfDaysInMonth = DateTime.DaysInMonth(year, month);
            var day = random.Next(1, noOfDaysInMonth + 1);

            generatedDate = new DateTime(year, month, day).Date;
        }
        return generatedDate;
    }
}
