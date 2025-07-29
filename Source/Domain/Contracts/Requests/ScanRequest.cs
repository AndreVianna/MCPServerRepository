using MCPHub.Domain.Entities;

namespace MCPHub.Domain.Contracts.Requests;

/// <summary>
/// Request for initiating a security scan
/// </summary>
public record ScanRequest {
    /// <summary>
    /// Type of security scan to perform
    /// </summary>
    public ScanType ScanType { get; init; }

    /// <summary>
    /// Additional options for the scan
    /// </summary>
    public Dictionary<string, object>? ScanOptions { get; init; }

    /// <summary>
    /// Whether to include detailed results in the response
    /// </summary>
    public bool IncludeDetailedResults { get; init; } = false;

    /// <summary>
    /// Optional callback URL for async scan results
    /// </summary>
    public string? CallbackUrl { get; init; }

    /// <summary>
    /// Priority level for the scan (1-10, 10 being highest)
    /// </summary>
    public int Priority { get; init; } = 5;

    /// <summary>
    /// Timeout for the scan in seconds
    /// </summary>
    public int TimeoutSeconds { get; init; } = 300;
}