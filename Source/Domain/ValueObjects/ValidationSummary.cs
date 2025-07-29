namespace MCPHub.Domain.ValueObjects;

/// <summary>
/// Represents the validation results for package manifest and content validation
/// </summary>
public class ValidationSummary {
    /// <summary>
    /// Indicates if the validation passed without errors
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// List of validation errors that prevent publication
    /// </summary>
    public IReadOnlyList<string> Errors { get; init; } = [];

    /// <summary>
    /// List of validation warnings that don't prevent publication but should be addressed
    /// </summary>
    public IReadOnlyList<string> Warnings { get; init; } = [];

    /// <summary>
    /// Validation context (e.g., "manifest", "content", "permissions")
    /// </summary>
    public string Context { get; init; } = string.Empty;

    /// <summary>
    /// Time taken to perform validation in milliseconds
    /// </summary>
    public long ValidationTimeMs { get; init; }

    public ValidationSummary() {
        IsValid = true;
        Errors = new List<string>().AsReadOnly();
        Warnings = new List<string>().AsReadOnly();
        Context = string.Empty;
        ValidationTimeMs = 0;
    } // For serialization

    public ValidationSummary(
        bool isValid,
        IEnumerable<string>? errors = null,
        IEnumerable<string>? warnings = null,
        string context = "",
        long validationTimeMs = 0) {
        IsValid = isValid;
        Errors = errors?.ToList().AsReadOnly() ?? new List<string>().AsReadOnly();
        Warnings = warnings?.ToList().AsReadOnly() ?? new List<string>().AsReadOnly();
        Context = context;
        ValidationTimeMs = validationTimeMs;
    }

    /// <summary>
    /// Creates a successful validation summary
    /// </summary>
    public static ValidationSummary Success(string context = "", long validationTimeMs = 0, IEnumerable<string>? warnings = null) => new ValidationSummary(true, null, warnings, context, validationTimeMs);

    /// <summary>
    /// Creates a failed validation summary
    /// </summary>
    public static ValidationSummary Failure(IEnumerable<string> errors, string context = "", long validationTimeMs = 0, IEnumerable<string>? warnings = null) => new ValidationSummary(false, errors, warnings, context, validationTimeMs);

    /// <summary>
    /// Creates a failed validation summary with a single error
    /// </summary>
    public static ValidationSummary Failure(string error, string context = "", long validationTimeMs = 0, IEnumerable<string>? warnings = null) => new ValidationSummary(false, [error], warnings, context, validationTimeMs);

    /// <summary>
    /// Combines multiple validation summaries into one
    /// </summary>
    public static ValidationSummary Combine(params ValidationSummary[] summaries) {
        if (summaries.Length == 0)
            return Success();

        var allErrors = summaries.SelectMany(s => s.Errors).ToList();
        var allWarnings = summaries.SelectMany(s => s.Warnings).ToList();
        var isValid = summaries.All(s => s.IsValid);
        var totalTime = summaries.Sum(s => s.ValidationTimeMs);
        var context = string.Join(", ", summaries.Where(s => !string.IsNullOrEmpty(s.Context)).Select(s => s.Context));

        return new ValidationSummary(isValid, allErrors, allWarnings, context, totalTime);
    }
}