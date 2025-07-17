namespace Domain.ValueObjects;

/// <summary>
/// Value object representing the result of a security scan
/// </summary>
public class SecurityScanResult
{
    public SecurityScanStatus Status { get; private set; }
    public int VulnerabilityCount { get; private set; }
    public SecurityScanSeverity HighestSeverity { get; private set; }
    public List<SecurityVulnerability> Vulnerabilities { get; private set; }
    public DateTime ScannedAt { get; private set; }
    public string ScannerVersion { get; private set; }
    public string? ScanLog { get; private set; }

    private SecurityScanResult() { } // For EF Core

    public SecurityScanResult(
        SecurityScanStatus status,
        List<SecurityVulnerability> vulnerabilities,
        string scannerVersion,
        string? scanLog = null)
    {
        Status = status;
        Vulnerabilities = vulnerabilities ?? new List<SecurityVulnerability>();
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

public enum SecurityScanStatus
{
    Pending,
    InProgress,
    Passed,
    Failed,
    Error
}

public enum SecurityScanSeverity
{
    None,
    Low,
    Medium,
    High,
    Critical
}

public class SecurityVulnerability
{
    public string Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public SecurityScanSeverity Severity { get; private set; }
    public string? CveId { get; private set; }
    public string? Reference { get; private set; }

    private SecurityVulnerability() { } // For EF Core

    public SecurityVulnerability(
        string id,
        string title,
        string description,
        SecurityScanSeverity severity,
        string? cveId = null,
        string? reference = null)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Severity = severity;
        CveId = cveId;
        Reference = reference;
    }
}