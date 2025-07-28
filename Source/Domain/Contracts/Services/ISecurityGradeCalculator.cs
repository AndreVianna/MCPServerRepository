using MCPHub.Domain.Entities;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.Contracts.Services;

/// <summary>
/// Interface for calculating security grades based on scan results
/// </summary>
public interface ISecurityGradeCalculator
{
    /// <summary>
    /// Calculates a security grade based on vulnerabilities found
    /// </summary>
    /// <param name="vulnerabilities">List of vulnerabilities</param>
    /// <param name="policy">Security policy to evaluate against</param>
    /// <returns>Security grade (A+ to F)</returns>
    string CalculateGrade(IEnumerable<SecurityVulnerability> vulnerabilities, SecurityPolicy? policy = null);
    
    /// <summary>
    /// Calculates a security grade based on scan results
    /// </summary>
    /// <param name="scanResults">List of security scan results</param>
    /// <param name="policy">Security policy to evaluate against</param>
    /// <returns>Security grade (A+ to F)</returns>
    string CalculateGrade(IEnumerable<SecurityScanResult> scanResults, SecurityPolicy? policy = null);
    
    /// <summary>
    /// Calculates a comprehensive security summary
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="scanResults">All scan results for the package</param>
    /// <param name="policy">Security policy to evaluate against</param>
    /// <returns>Security scan summary</returns>
    SecurityScanSummary CalculateSummary(
        Guid packageId, 
        IEnumerable<SecurityScanResult> scanResults, 
        SecurityPolicy? policy = null);
    
    /// <summary>
    /// Gets the grade threshold configuration
    /// </summary>
    /// <returns>Dictionary of grade thresholds</returns>
    Dictionary<string, GradeThreshold> GetGradeThresholds();
    
    /// <summary>
    /// Validates if a security grade meets the minimum required grade
    /// </summary>
    /// <param name="actualGrade">The calculated grade</param>
    /// <param name="minimumGrade">The minimum required grade</param>
    /// <returns>True if the actual grade meets or exceeds the minimum</returns>
    bool MeetsMinimumGrade(string actualGrade, string minimumGrade);
}

/// <summary>
/// Configuration for security grade thresholds
/// </summary>
public record GradeThreshold
{
    /// <summary>
    /// Maximum number of critical issues allowed for this grade
    /// </summary>
    public int MaxCriticalIssues { get; init; }
    
    /// <summary>
    /// Maximum number of high issues allowed for this grade
    /// </summary>
    public int MaxHighIssues { get; init; }
    
    /// <summary>
    /// Maximum number of medium issues allowed for this grade
    /// </summary>
    public int MaxMediumIssues { get; init; }
    
    /// <summary>
    /// Maximum total score allowed for this grade
    /// </summary>
    public int MaxTotalScore { get; init; }
    
    /// <summary>
    /// Description of what this grade represents
    /// </summary>
    public string Description { get; init; } = string.Empty;
}