using System.Diagnostics.CodeAnalysis;

using MCPHub.Domain.Common;

namespace MCPHub.Domain.Entities;

/// <summary>
/// Represents a security event for comprehensive security monitoring and analysis
/// </summary>
public class SecurityEvent : BaseEntity {
    /// <summary>
    /// Type of security event
    /// </summary>
    public SecurityEventType EventType { get; set; }

    /// <summary>
    /// Severity level of the security event
    /// </summary>
    public SecurityEventSeverity Severity { get; set; }

    /// <summary>
    /// Event message or description
    /// </summary>
    [MaxLength(1024)]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Source system or component that generated the event
    /// </summary>
    [MaxLength(128)]
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// User ID associated with the event (if applicable)
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Username associated with the event (denormalized for performance)
    /// </summary>
    [MaxLength(256)]
    public string? Username { get; set; }

    /// <summary>
    /// Resource type involved in the event
    /// </summary>
    [MaxLength(64)]
    public string? ResourceType { get; set; }

    /// <summary>
    /// Resource ID involved in the event
    /// </summary>
    public Guid? ResourceId { get; set; }

    /// <summary>
    /// Resource name involved in the event (denormalized for performance)
    /// </summary>
    [MaxLength(256)]
    public string? ResourceName { get; set; }

    /// <summary>
    /// IP address from which the event originated
    /// </summary>
    [MaxLength(45)] // IPv6 max length
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent string from the request
    /// </summary>
    [MaxLength(512)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Session ID associated with the event
    /// </summary>
    [MaxLength(128)]
    public string? SessionId { get; set; }

    /// <summary>
    /// Correlation ID for linking related events
    /// </summary>
    public Guid? CorrelationId { get; set; }

    /// <summary>
    /// Additional metadata as JSON
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = [];

    /// <summary>
    /// Whether this event has been processed by security systems
    /// </summary>
    public bool IsProcessed { get; set; } = false;

    /// <summary>
    /// Processing result or status
    /// </summary>
    [MaxLength(256)]
    public string? ProcessingResult { get; set; }

    /// <summary>
    /// Whether this event triggered an alert
    /// </summary>
    public bool TriggeredAlert { get; set; } = false;

    /// <summary>
    /// Risk score calculated for this event (0-100)
    /// </summary>
    public int RiskScore { get; set; } = 0;

    /// <summary>
    /// Tags associated with this event for categorization
    /// </summary>
    public List<string> Tags { get; set; } = [];

    /// <summary>
    /// Hash of the event content for integrity verification
    /// </summary>
    [MaxLength(64)]
    public string? ContentHash { get; set; }

    /// <summary>
    /// Whether this event is archived
    /// </summary>
    public bool IsArchived { get; set; } = false;

    /// <summary>
    /// Date when the event was archived
    /// </summary>
    public DateTimeOffset? ArchivedAt { get; set; }

    /// <summary>
    /// Navigation property to the related user
    /// </summary>
    public ApplicationUser? User { get; set; }

    private SecurityEvent() { } // For EF Core

    [SetsRequiredMembers]
    public SecurityEvent(
        SecurityEventType eventType,
        SecurityEventSeverity severity,
        string message,
        string source,
        Guid? userId = null) {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Event message cannot be null or empty", nameof(message));
        if (string.IsNullOrWhiteSpace(source))
            throw new ArgumentException("Event source cannot be null or empty", nameof(source));

        EventType = eventType;
        Severity = severity;
        Message = message;
        Source = source;
        UserId = userId;

        // Calculate initial risk score based on severity
        RiskScore = CalculateRiskScore(severity, eventType);

        AuditTrail.Add(new AuditEntry {
            Action = "Security Event Created",
            UserId = userId ?? Guid.Empty,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Updates the event with additional context information
    /// </summary>
    /// <param name="resourceType">Resource type</param>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="resourceName">Resource name</param>
    /// <param name="ipAddress">IP address</param>
    /// <param name="userAgent">User agent</param>
    /// <param name="sessionId">Session ID</param>
    public void UpdateContext(
        string? resourceType = null,
        Guid? resourceId = null,
        string? resourceName = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? sessionId = null) {

        ResourceType = resourceType;
        ResourceId = resourceId;
        ResourceName = resourceName;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        SessionId = sessionId;

        AuditTrail.Add(new AuditEntry {
            Action = "Security Event Context Updated",
            UserId = UserId ?? Guid.Empty,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Adds metadata to the event
    /// </summary>
    /// <param name="key">Metadata key</param>
    /// <param name="value">Metadata value</param>
    public void AddMetadata(string key, object value) {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Metadata key cannot be null or empty", nameof(key));

        Metadata[key] = value;

        AuditTrail.Add(new AuditEntry {
            Action = "Security Event Metadata Added",
            UserId = UserId ?? Guid.Empty,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Adds a tag to the event
    /// </summary>
    /// <param name="tag">Tag to add</param>
    public void AddTag(string tag) {
        if (string.IsNullOrWhiteSpace(tag))
            throw new ArgumentException("Tag cannot be null or empty", nameof(tag));

        if (!Tags.Contains(tag, StringComparer.OrdinalIgnoreCase)) {
            Tags.Add(tag);

            AuditTrail.Add(new AuditEntry {
                Action = "Security Event Tag Added",
                UserId = UserId ?? Guid.Empty,
                DateTime = DateTimeOffset.UtcNow
            });
        }
    }

    /// <summary>
    /// Marks the event as processed
    /// </summary>
    /// <param name="result">Processing result</param>
    /// <param name="processedBy">User who processed the event</param>
    public void MarkAsProcessed(string result, Guid? processedBy = null) {
        IsProcessed = true;
        ProcessingResult = result;

        AuditTrail.Add(new AuditEntry {
            Action = "Security Event Processed",
            UserId = processedBy ?? Guid.Empty,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Sets the correlation ID for linking related events
    /// </summary>
    /// <param name="correlationId">Correlation ID</param>
    public void SetCorrelationId(Guid correlationId) {
        CorrelationId = correlationId;

        AuditTrail.Add(new AuditEntry {
            Action = "Security Event Correlated",
            UserId = UserId ?? Guid.Empty,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Updates the risk score of the event
    /// </summary>
    /// <param name="newRiskScore">New risk score (0-100)</param>
    /// <param name="reason">Reason for the update</param>
    /// <param name="updatedBy">User who updated the score</param>
    public void UpdateRiskScore(int newRiskScore, string reason, Guid? updatedBy = null) {
        if (newRiskScore < 0 || newRiskScore > 100)
            throw new ArgumentOutOfRangeException(nameof(newRiskScore), "Risk score must be between 0 and 100");

        var oldScore = RiskScore;
        RiskScore = newRiskScore;

        AddMetadata("RiskScoreUpdate", new {
            OldScore = oldScore,
            NewScore = newRiskScore,
            Reason = reason,
            UpdatedAt = DateTimeOffset.UtcNow
        });

        AuditTrail.Add(new AuditEntry {
            Action = "Security Event Risk Score Updated",
            UserId = updatedBy ?? Guid.Empty,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Marks the event as having triggered an alert
    /// </summary>
    /// <param name="triggeredBy">User or system that triggered the alert</param>
    public void MarkAlertTriggered(Guid? triggeredBy = null) {
        TriggeredAlert = true;

        AuditTrail.Add(new AuditEntry {
            Action = "Security Event Alert Triggered",
            UserId = triggeredBy ?? Guid.Empty,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Archives the event
    /// </summary>
    /// <param name="archivedBy">User who archived the event</param>
    public void Archive(Guid? archivedBy = null) {
        IsArchived = true;
        ArchivedAt = DateTimeOffset.UtcNow;

        AuditTrail.Add(new AuditEntry {
            Action = "Security Event Archived",
            UserId = archivedBy ?? Guid.Empty,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Generates a content hash for integrity verification
    /// </summary>
    public void GenerateContentHash() {
        var content = $"{EventType}|{Severity}|{Message}|{Source}|{UserId}|{ResourceType}|{ResourceId}|{IpAddress}";
        ContentHash = ComputeHash(content);
    }

    /// <summary>
    /// Verifies the integrity of the event using the content hash
    /// </summary>
    /// <returns>True if the event integrity is valid</returns>
    public bool VerifyIntegrity() {
        if (string.IsNullOrEmpty(ContentHash))
            return false;

        var content = $"{EventType}|{Severity}|{Message}|{Source}|{UserId}|{ResourceType}|{ResourceId}|{IpAddress}";
        var computedHash = ComputeHash(content);
        return ContentHash.Equals(computedHash, StringComparison.OrdinalIgnoreCase);
    }

    private static int CalculateRiskScore(SecurityEventSeverity severity, SecurityEventType eventType) {
        var baseScore = severity switch {
            SecurityEventSeverity.Informational => 10,
            SecurityEventSeverity.Low => 25,
            SecurityEventSeverity.Medium => 50,
            SecurityEventSeverity.High => 75,
            SecurityEventSeverity.Critical => 90,
            _ => 10
        };

        // Adjust score based on event type
        var typeMultiplier = eventType switch {
            SecurityEventType.ThreatDetected => 1.2,
            SecurityEventType.PolicyViolation => 1.1,
            SecurityEventType.IncidentCreated => 1.3,
            SecurityEventType.DataAccess => 1.0,
            SecurityEventType.Authorization => 0.9,
            SecurityEventType.Authentication => 0.9,
            _ => 1.0
        };

        return Math.Min(100, (int)(baseScore * typeMultiplier));
    }

    private static string ComputeHash(string input) {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hashedBytes).ToLowerInvariant();
    }
}

/// <summary>
/// Security event types
/// </summary>
public enum SecurityEventType {
    Authentication = 1,
    Authorization = 2,
    DataAccess = 3,
    DataModification = 4,
    PolicyViolation = 5,
    ThreatDetected = 6,
    IncidentCreated = 7,
    IncidentResolved = 8,
    ConfigurationChange = 9,
    ApiKeyCreated = 10,
    ApiKeyRevoked = 11,
    PackagePublished = 12,
    PackageDeleted = 13,
    ServerRegistered = 14,
    ServerDeregistered = 15,
    ScanCompleted = 16,
    TrustTierUpdated = 17,
    ComplianceAudit = 18,
    UserActivity = 19,
    SystemEvent = 20
}

/// <summary>
/// Security event severity levels
/// </summary>
public enum SecurityEventSeverity {
    Informational = 0,
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}