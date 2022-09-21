using System.Text.RegularExpressions;

namespace Acs.Messaging.Sample.Shared;

public static class Utils
{
    public static string HidePhoneNumber(string? value) => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Length <= 7 ? value : $"{Take(value, 4)}..{Last(value, 3)}";

    public static string? FormatPhoneNumber(string? value, string? format = "+# (###) ###-####")
    {
        if (value == null)
            return null;

        Regex regex = new(@"[^\d]");
        value = regex.Replace(value, "");

        if (value.Length > 0)
            return Convert.ToInt64(value).ToString(format);

        return value;
    }

    public static string Take(string value, int count) => value.Length <= count ? value : value[..count];

    public static string Last(string value, int count) => value.Length <= count ? value : value[^count..];
}