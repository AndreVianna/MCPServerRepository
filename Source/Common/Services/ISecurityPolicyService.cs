namespace MCPHub.Common.Services;

/// <summary>
/// Security policy management service for enterprise-grade policy enforcement
/// Supports: In-Memory → Database → Enterprise Policy Manager
/// </summary>
public interface ISecurityPolicyService {
    /// <summary>
    /// Creates a new security policy
    /// </summary>
    /// <param name="request">Policy creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created policy result</returns>
    Task<SecurityPolicyResult> CreatePolicyAsync(CreateSecurityPolicyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing security policy
    /// </summary>
    /// <param name="policyId">Policy identifier</param>
    /// <param name="request">Policy update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated policy result</returns>
    Task<SecurityPolicyResult> UpdatePolicyAsync(Guid policyId, UpdateSecurityPolicyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a security policy
    /// </summary>
    /// <param name="policyId">Policy identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion result</returns>
    Task<SecurityPolicyDeletionResult> DeletePolicyAsync(Guid policyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a security policy by ID
    /// </summary>
    /// <param name="policyId">Policy identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Policy details or null if not found</returns>
    Task<SecurityPolicyDetails?> GetPolicyAsync(Guid policyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all security policies for an organization
    /// </summary>
    /// <param name="organizationId">Organization identifier (null for global policies)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of policies</returns>
    Task<IReadOnlyList<SecurityPolicyDetails>> GetPoliciesAsync(Guid? organizationId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Activates or deactivates a security policy
    /// </summary>
    /// <param name="policyId">Policy identifier</param>
    /// <param name="isActive">Whether to activate or deactivate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Activation result</returns>
    Task<SecurityPolicyActivationResult> SetPolicyActiveAsync(Guid policyId, bool isActive, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a policy as the default for new packages
    /// </summary>
    /// <param name="policyId">Policy identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Default policy result</returns>
    Task<SecurityPolicyDefaultResult> SetDefaultPolicyAsync(Guid policyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Evaluates if a package meets the specified policy requirements
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="policyId">Policy identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Policy evaluation result</returns>
    Task<SecurityPolicyEvaluationResult> EvaluatePolicyAsync(Guid packageId, Guid policyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets policy compliance statistics
    /// </summary>
    /// <param name="organizationId">Organization identifier (null for global stats)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Policy compliance statistics</returns>
    Task<SecurityPolicyComplianceStats> GetComplianceStatsAsync(Guid? organizationId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates policy configuration before creation/update
    /// </summary>
    /// <param name="policyConfiguration">Policy configuration to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result</returns>
    Task<SecurityPolicyValidationResult> ValidatePolicyAsync(SecurityPolicyConfiguration policyConfiguration, CancellationToken cancellationToken = default);
}

/// <summary>
/// Request for creating a new security policy
/// </summary>
public record CreateSecurityPolicyRequest(
    string Name,
    string Description,
    SecurityPolicyConfiguration Configuration,
    Guid? OrganizationId = null,
    bool IsActive = true);

/// <summary>
/// Request for updating an existing security policy
/// </summary>
public record UpdateSecurityPolicyRequest(
    string? Name = null,
    string? Description = null,
    SecurityPolicyConfiguration? Configuration = null,
    bool? IsActive = null);

/// <summary>
/// Security policy configuration
/// </summary>
public record SecurityPolicyConfiguration(
    SecurityScanSeverity MaxAllowedSeverity,
    IReadOnlyList<string> RequiredAnalyzers,
    IReadOnlyList<string> BlockedCapabilities,
    IDictionary<string, object> Rules,
    TimeSpan? MaxAge = null,
    bool RequireCodeSigning = false,
    bool RequireTrustedPublisher = false);

/// <summary>
/// Security policy result
/// </summary>
public record SecurityPolicyResult(
    Guid PolicyId,
    string Name,
    string Description,
    SecurityPolicyConfiguration Configuration,
    bool IsActive,
    bool IsDefault,
    Guid? OrganizationId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

/// <summary>
/// Security policy deletion result
/// </summary>
public record SecurityPolicyDeletionResult(
    bool Success,
    string? ErrorMessage = null);

/// <summary>
/// Detailed security policy information
/// </summary>
public record SecurityPolicyDetails(
    Guid PolicyId,
    string Name,
    string Description,
    SecurityPolicyConfiguration Configuration,
    bool IsActive,
    bool IsDefault,
    Guid? OrganizationId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    int PackageCount,
    int ComplianceCount,
    int ViolationCount);

/// <summary>
/// Security policy activation result
/// </summary>
public record SecurityPolicyActivationResult(
    bool Success,
    bool IsActive,
    string? ErrorMessage = null);

/// <summary>
/// Security policy default setting result
/// </summary>
public record SecurityPolicyDefaultResult(
    bool Success,
    Guid? PreviousDefaultPolicyId = null,
    string? ErrorMessage = null);

/// <summary>
/// Security policy evaluation result
/// </summary>
public record SecurityPolicyEvaluationResult(
    bool IsCompliant,
    SecurityScanSeverity HighestSeverity,
    IReadOnlyList<string> Violations,
    IReadOnlyList<string> Warnings,
    IDictionary<string, object> Details,
    DateTimeOffset EvaluatedAt);

/// <summary>
/// Security policy compliance statistics
/// </summary>
public record SecurityPolicyComplianceStats(
    int TotalPolicies,
    int ActivePolicies,
    int InactivePolicies,
    int TotalPackages,
    int CompliantPackages,
    int NonCompliantPackages,
    double CompliancePercentage,
    IDictionary<SecurityScanSeverity, int> ViolationsBySeverity,
    DateTimeOffset GeneratedAt);

/// <summary>
/// Security policy validation result
/// </summary>
public record SecurityPolicyValidationResult(
    bool IsValid,
    IReadOnlyList<string> Errors,
    IReadOnlyList<string> Warnings);

/// <summary>
/// Security scan severity levels for policy configuration
/// </summary>
public enum SecurityScanSeverity {
    None = 0,
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4,
}