using MCPHub.Domain.Entities;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// Result of a package publishing operation
/// </summary>
public class PublishResult {
    /// <summary>
    /// Indicates if the publishing operation was successful
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// List of errors that occurred during publishing
    /// </summary>
    public IReadOnlyList<string> Errors { get; init; } = [];

    /// <summary>
    /// List of warnings from the publishing process
    /// </summary>
    public IReadOnlyList<string> Warnings { get; init; } = [];

    /// <summary>
    /// The published package (if successful)
    /// </summary>
    public Package? Package { get; init; }

    /// <summary>
    /// The published package version (if successful and this was a version publish)
    /// </summary>
    public PackageVersion? PackageVersion { get; init; }

    /// <summary>
    /// Comprehensive validation summary
    /// </summary>
    public ValidationSummary ValidationSummary { get; init; } = new ValidationSummary();

    /// <summary>
    /// Publication timestamp
    /// </summary>
    public DateTimeOffset PublishedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Total time taken for the publishing operation in milliseconds
    /// </summary>
    public long PublishTimeMs { get; init; }

    /// <summary>
    /// Storage URL where the package was uploaded
    /// </summary>
    public string? StorageUrl { get; init; }

    private PublishResult() { } // For serialization

    public PublishResult(
        bool success,
        IEnumerable<string>? errors = null,
        IEnumerable<string>? warnings = null,
        Package? package = null,
        PackageVersion? packageVersion = null,
        ValidationSummary? validationSummary = null,
        long publishTimeMs = 0,
        string? storageUrl = null) {
        Success = success;
        Errors = errors?.ToList().AsReadOnly() ?? new List<string>().AsReadOnly();
        Warnings = warnings?.ToList().AsReadOnly() ?? new List<string>().AsReadOnly();
        Package = package;
        PackageVersion = packageVersion;
        ValidationSummary = validationSummary ?? new ValidationSummary();
        PublishTimeMs = publishTimeMs;
        StorageUrl = storageUrl;
    }

    /// <summary>
    /// Creates a successful publish result
    /// </summary>
    public static PublishResult CreateSuccess(
        Package? package = null,
        PackageVersion? packageVersion = null,
        ValidationSummary? validationSummary = null,
        long publishTimeMs = 0,
        string? storageUrl = null,
        IEnumerable<string>? warnings = null) => new(
            true,
            null,
            warnings,
            package,
            packageVersion,
            validationSummary,
            publishTimeMs,
            storageUrl);

    /// <summary>
    /// Creates a failed publish result
    /// </summary>
    public static PublishResult CreateFailure(
        IEnumerable<string> errors,
        ValidationSummary? validationSummary = null,
        long publishTimeMs = 0,
        IEnumerable<string>? warnings = null) => new(
            false,
            errors,
            warnings,
            null,
            null,
            validationSummary,
            publishTimeMs,
            null);

    /// <summary>
    /// Creates a failed publish result with a single error
    /// </summary>
    public static PublishResult CreateFailure(
        string error,
        ValidationSummary? validationSummary = null,
        long publishTimeMs = 0,
        IEnumerable<string>? warnings = null) => new(
            false,
            [error],
            warnings,
            null,
            null,
            validationSummary,
            publishTimeMs,
            null);
}