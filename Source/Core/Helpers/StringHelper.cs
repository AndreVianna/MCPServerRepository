using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Helpers;

/// <summary>
/// Helper class for string operations
/// </summary>
public static partial class StringHelper {
    [GeneratedRegex(@"([a-z0-9])([A-Z])")]
    private static partial Regex KebabCaseRegex();

    [GeneratedRegex(@"[^a-z0-9\s-]")]
    private static partial Regex SlugInvalidCharRegex();
    [GeneratedRegex(@"\s+")]
    private static partial Regex SlugWhitespaceRegex();
    [GeneratedRegex(@"-+")]
    private static partial Regex SlugHyphenRegex();

    public static string Truncate(this string value, int maxLength) => string.IsNullOrEmpty(value) ? value : value.Length <= maxLength ? value : value[..maxLength];

    public static string ToKebabCase(this string value)
        => string.IsNullOrEmpty(value) ? value
        : KebabCaseRegex().Replace(value, "$1-$2").ToLowerInvariant();

    public static string ToPascalCase(this string value) {
        if (string.IsNullOrEmpty(value))
            return value;

        var words = value.Split(['-', '_', ' '], StringSplitOptions.RemoveEmptyEntries);
        var result = new StringBuilder();
        foreach (var word in words) {
            if (word.Length > 0) {
                result.Append(char.ToUpperInvariant(word[0]));
                if (word.Length > 1)
                    result.Append(word[1..].ToLowerInvariant());
            }
        }

        return result.ToString();
    }

    public static bool IsValidEmail(this string email) {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try {
            var mail = new MailAddress(email);
            return mail.Address == email;
        }
        catch {
            return false;
        }
    }

    public static string GenerateSlug(this string value) {
        if (string.IsNullOrEmpty(value))
            return value;

        // Convert to lowercase and replace invalid characters
        value = value.ToLowerInvariant();
        value = SlugInvalidCharRegex().Replace(value, "");
        value = SlugWhitespaceRegex().Replace(value, "-");
        value = SlugHyphenRegex().Replace(value, "-");
        value = value.Trim('-');

        return value;
    }
}