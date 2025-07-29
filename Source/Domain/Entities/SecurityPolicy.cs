using System.Diagnostics.CodeAnalysis;

using MCPHub.Domain.Common;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.Entities;

/// <summary>
/// Represents a security policy that defines scanning rules and thresholds
/// </summary>
public class SecurityPolicy : BaseEntity {
    /// <summary>
    /// Human-readable name of the security policy
    /// </summary>
    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of what this policy enforces
    /// </summary>
    [MaxLength(512)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Policy rules as a JSON object
    /// </summary>
    public Dictionary<string, object> Rules { get; set; } = [];

    /// <summary>
    /// Maximum allowed severity level for packages to pass this policy
    /// </summary>
    public SecurityScanSeverity MaxAllowedSeverity { get; set; } = SecurityScanSeverity.Medium;

    /// <summary>
    /// List of required analyzers that must be run
    /// </summary>
    public List<string> RequiredAnalyzers { get; set; } = [];

    /// <summary>
    /// List of capabilities that are blocked by this policy
    /// </summary>
    public List<string> BlockedCapabilities { get; set; } = [];

    /// <summary>
    /// Whether this policy is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether this is the default policy for new packages
    /// </summary>
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// Organization ID this policy belongs to (null for global policies)
    /// </summary>
    public Guid? OrganizationId { get; set; }

    private SecurityPolicy() { } // For EF Core

    [SetsRequiredMembers]
    public SecurityPolicy(
        string name,
        string description,
        SecurityScanSeverity maxAllowedSeverity = SecurityScanSeverity.Medium) {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Policy name cannot be null or empty", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Policy description cannot be null or empty", nameof(description));

        Name = name;
        Description = description;
        MaxAllowedSeverity = maxAllowedSeverity;

        AuditTrail.Add(new AuditEntry {
            Action = "Security Policy Created",
            UserId = Guid.Empty, // Will be set by the service
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Updates the policy rules
    /// </summary>
    /// <param name="rules">New policy rules</param>
    /// <param name="userId">ID of user making the change</param>
    public void UpdateRules(Dictionary<string, object> rules, Guid userId) {
        Rules = rules ?? [];
        AuditTrail.Add(new AuditEntry {
            Action = "Security Policy Rules Updated",
            UserId = userId,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Activates or deactivates the policy
    /// </summary>
    /// <param name="isActive">Whether the policy should be active</param>
    /// <param name="userId">ID of user making the change</param>
    public void SetActive(bool isActive, Guid userId) {
        if (IsActive == isActive)
            return;

        IsActive = isActive;
        AuditTrail.Add(new AuditEntry {
            Action = $"Security Policy {(isActive ? "Activated" : "Deactivated")}",
            UserId = userId,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Sets this policy as the default policy
    /// </summary>
    /// <param name="userId">ID of user making the change</param>
    public void SetAsDefault(Guid userId) {
        if (IsDefault)
            return;

        IsDefault = true;
        AuditTrail.Add(new AuditEntry {
            Action = "Security Policy Set as Default",
            UserId = userId,
            DateTime = DateTimeOffset.UtcNow
        });
    }
}