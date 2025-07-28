using System.Diagnostics.CodeAnalysis;

namespace MCPHub.Domain.ValueObjects;

/// <summary>
/// Summary of security scan results for a package
/// </summary>
public record SecurityScanSummary
{
    /// <summary>
    /// Overall severity level based on highest finding
    /// </summary>
    public SecurityScanSeverity OverallSeverity { get; init; }
    
    /// <summary>
    /// Number of critical severity issues found
    /// </summary>
    public int CriticalIssues { get; init; }
    
    /// <summary>
    /// Number of high severity issues found
    /// </summary>
    public int HighIssues { get; init; }
    
    /// <summary>
    /// Number of medium severity issues found
    /// </summary>
    public int MediumIssues { get; init; }
    
    /// <summary>
    /// Number of low severity issues found
    /// </summary>
    public int LowIssues { get; init; }
    
    /// <summary>
    /// Date and time of the last security scan
    /// </summary>
    public DateTimeOffset LastScanDate { get; init; }
    
    /// <summary>
    /// Security grade (A+ to F) based on scan results
    /// </summary>
    [MaxLength(2)]
    public string SecurityGrade { get; init; } = "F";
    
    /// <summary>
    /// Brief description of the security status
    /// </summary>
    [MaxLength(256)]
    public string? StatusDescription { get; init; }
    
    /// <summary>
    /// Total number of issues found across all severity levels
    /// </summary>
    public int TotalIssues => CriticalIssues + HighIssues + MediumIssues + LowIssues;
    
    /// <summary>
    /// Whether the package passes basic security requirements
    /// </summary>
    public bool PassesBasicSecurity => CriticalIssues == 0 && HighIssues == 0;
    
    private SecurityScanSummary() { } // For EF Core
    
    [SetsRequiredMembers]
    public SecurityScanSummary(
        SecurityScanSeverity overallSeverity,
        int criticalIssues,
        int highIssues,
        int mediumIssues,
        int lowIssues,
        DateTimeOffset lastScanDate,
        string securityGrade,
        string? statusDescription = null)
    {
        if (criticalIssues < 0) throw new ArgumentException("Critical issues count cannot be negative", nameof(criticalIssues));
        if (highIssues < 0) throw new ArgumentException("High issues count cannot be negative", nameof(highIssues));
        if (mediumIssues < 0) throw new ArgumentException("Medium issues count cannot be negative", nameof(mediumIssues));
        if (lowIssues < 0) throw new ArgumentException("Low issues count cannot be negative", nameof(lowIssues));
        if (string.IsNullOrWhiteSpace(securityGrade)) throw new ArgumentException("Security grade cannot be null or empty", nameof(securityGrade));
        
        OverallSeverity = overallSeverity;
        CriticalIssues = criticalIssues;
        HighIssues = highIssues;
        MediumIssues = mediumIssues;
        LowIssues = lowIssues;
        LastScanDate = lastScanDate;
        SecurityGrade = securityGrade;
        StatusDescription = statusDescription;
    }
}