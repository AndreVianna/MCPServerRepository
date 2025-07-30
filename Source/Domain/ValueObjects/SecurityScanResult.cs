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

    /// <summary>
    /// Trust score calculated based on scan results (0-1000)
    /// </summary>
    public int TrustScore => CalculateTrustScore();

    /// <summary>
    /// Overall security grade (A, B, C, D, F) based on trust score
    /// </summary>
    public string OverallGrade => CalculateGrade();

    /// <summary>
    /// Overall security score (0-10) for UI display
    /// </summary>
    public double OverallScore => TrustScore / 100.0;

    /// <summary>
    /// Last scan date for UI display
    /// </summary>
    public DateTime LastScanDate => ScannedAt;

    /// <summary>
    /// Scan duration (estimated based on vulnerability count)
    /// </summary>
    public TimeSpan ScanDuration => TimeSpan.FromMinutes(Math.Max(1, VulnerabilityCount * 0.1 + 2));

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

    private int CalculateTrustScore() {
        if (Status != SecurityScanStatus.Passed)
            return 0;

        if (VulnerabilityCount == 0)
            return 1000;

        // Calculate score based on vulnerability severity
        var score = 1000;
        foreach (var vulnerability in Vulnerabilities) {
            score -= vulnerability.Severity switch {
                SecurityScanSeverity.Critical => 300,
                SecurityScanSeverity.High => 150,
                SecurityScanSeverity.Medium => 75,
                SecurityScanSeverity.Low => 25,
                _ => 0,
            };
        }

        return Math.Max(0, score);
    }

    private string CalculateGrade() => TrustScore switch {
        >= 900 => "A",
        >= 800 => "B",
        >= 700 => "C",
        >= 600 => "D",
        _ => "F",
    };
}