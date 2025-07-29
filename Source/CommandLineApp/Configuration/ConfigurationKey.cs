namespace MCPHub.CommandLineApp.Configuration;

/// <summary>
/// Represents a configuration key with metadata for validation and display
/// </summary>
public class ConfigurationKey {
    public required string Key { get; init; }
    public required string Description { get; init; }
    public required Type ValueType { get; init; }
    public object? DefaultValue { get; init; }
    public bool IsSecret { get; init; }
    public bool IsReadOnly { get; init; }
    public string[]? ValidValues { get; init; }
    public object? MinValue { get; init; }
    public object? MaxValue { get; init; }
    public string? Pattern { get; init; }
    public string? DisplayName { get; init; }
    public string? Category { get; init; }

    public string GetDisplayName() => DisplayName ?? Key;

    public bool IsValid(object? value) {
        if (value == null)
            return true;

        // Type validation
        if (!ValueType.IsAssignableFrom(value.GetType()))
            return false;

        // Valid values validation
        if (ValidValues != null && !ValidValues.Contains(value.ToString(), StringComparer.OrdinalIgnoreCase))
            return false;

        // Range validation for numeric types
        if (MinValue != null && value is IComparable minComparable && minComparable.CompareTo(MinValue) < 0)
            return false;

        if (MaxValue != null && value is IComparable maxComparable && maxComparable.CompareTo(MaxValue) > 0)
            return false;

        // Pattern validation for strings
        if (!string.IsNullOrEmpty(Pattern) && value is string stringValue) {
            return System.Text.RegularExpressions.Regex.IsMatch(stringValue, Pattern);
        }

        return true;
    }
}