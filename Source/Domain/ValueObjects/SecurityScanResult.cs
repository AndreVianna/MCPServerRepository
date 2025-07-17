namespace Domain.ValueObjects;

/// <summary>
/// Value object representing the result of a security scan
/// </summary>
public class SecurityScanResult {
    public SecurityScanStatus Status { get; set; }
    public int VulnerabilityCount { get; set; }
    public SecurityScanSeverity HighestSeverity { get; set; }
    public List<SecurityVulnerability> Vulnerabilities { get; set; } = [];
    public DateTime ScannedAt { get; set; }
    [MaxLength(32)]
    public string ScannerVersion { get; set; } = string.Empty;
    [MaxLength(4096)]
    public string? ScanLog { get; set; }

    private SecurityScanResult() { } // For EF Core

    public SecurityScanResult(
        SecurityScanStatus status,
        List<SecurityVulnerability> vulnerabilities,
        string scannerVersion,
        string? scanLog = null) {
        Status = status;
        Vulnerabilities = vulnerabilities ?? [];
        VulnerabilityCount = Vulnerabilities.Count;
        HighestSeverity = Vulnerabilities.Count > 0
            ? Vulnerabilities.Max(v => v.Severity)
            : SecurityScanSeverity.None;
        ScannedAt = DateTime.UtcNow;
        ScannerVersion = scannerVersion ?? throw new ArgumentNullException(nameof(scannerVersion));
        ScanLog = scanLog;
    }

    public bool IsClean => Status == SecurityScanStatus.Passed && VulnerabilityCount == 0;
    public bool HasCriticalVulnerabilities => Vulnerabilities.Any(v => v.Severity == SecurityScanSeverity.Critical);
    public bool HasHighVulnerabilities => Vulnerabilities.Any(v => v.Severity == SecurityScanSeverity.High);
}