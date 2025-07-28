using System.Diagnostics.CodeAnalysis;

namespace MCPHub.Domain.ValueObjects;

/// <summary>
/// Value object representing the result of a security scan
/// </summary>
public record SecurityScanResult {
    public SecurityScanStatus Status { get; init; }
    public int VulnerabilityCount { get; init; }
    public SecurityScanSeverity HighestSeverity { get; init; }
    public List<SecurityVulnerability> Vulnerabilities { get; init; } = [];
    public DateTime ScannedAt { get; init; }
    [MaxLength(32)]
    public string ScannerVersion { get; init; } = string.Empty;
    [MaxLength(4096)]
    public string? ScanLog { get; init; }

    private SecurityScanResult() { } // For EF Core

    [SetsRequiredMembers]
    public SecurityScanResult(
        SecurityScanStatus status,
        List<SecurityVulnerability> vulnerabilities,
        string scannerVersion,
        string? scanLog = null) {
        ArgumentException.ThrowIfNullOrWhiteSpace(scannerVersion);

        Status = status;
        Vulnerabilities = vulnerabilities ?? [];
        VulnerabilityCount = Vulnerabilities.Count;
        HighestSeverity = Vulnerabilities.Count > 0
            ? Vulnerabilities.Max(v => v.Severity)
            : SecurityScanSeverity.None;
        ScannedAt = DateTime.UtcNow;
        ScannerVersion = scannerVersion;
        ScanLog = scanLog;
    }

    public bool IsClean => Status == SecurityScanStatus.Passed && VulnerabilityCount == 0;
    public bool HasCriticalVulnerabilities => Vulnerabilities.Any(v => v.Severity == SecurityScanSeverity.Critical);
    public bool HasHighVulnerabilities => Vulnerabilities.Any(v => v.Severity == SecurityScanSeverity.High);
}