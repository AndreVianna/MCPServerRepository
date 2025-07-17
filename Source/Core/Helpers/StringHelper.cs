using System.Text;
using System.Text.RegularExpressions;

namespace Core.Helpers;

/// <summary>
/// Helper class for string operations
/// </summary>
public static class StringHelper
{
    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        
        return value.Length <= maxLength ? value : value.Substring(0, maxLength);
    }

    public static string ToKebabCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return System.Text.RegularExpressions.Regex.Replace(value, @"([a-z0-9])([A-Z])", "$1-$2").ToLowerInvariant();
    }

    public static string ToPascalCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var words = value.Split(new[] { '-', '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var result = new StringBuilder();
        
        foreach (var word in words)
        {
            if (word.Length > 0)
            {
                result.Append(char.ToUpperInvariant(word[0]));
                if (word.Length > 1)
                    result.Append(word.Substring(1).ToLowerInvariant());
            }
        }
        
        return result.ToString();
    }

    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static string GenerateSlug(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        // Convert to lowercase and replace invalid characters
        value = value.ToLowerInvariant();
        value = System.Text.RegularExpressions.Regex.Replace(value, @"[^a-z0-9\s-]", "");
        value = System.Text.RegularExpressions.Regex.Replace(value, @"\s+", "-");
        value = System.Text.RegularExpressions.Regex.Replace(value, @"-+", "-");
        value = value.Trim('-');
        
        return value;
    }
}