using System.Diagnostics.CodeAnalysis;

using MCPHub.Domain.Common;

namespace MCPHub.Domain.Entities;

/// <summary>
/// Represents an API key for secure API access with scoping and permissions
/// </summary>
public class ApiKey : BaseEntity {
    /// <summary>
    /// Human-readable name for the API key
    /// </summary>
    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the API key's purpose
    /// </summary>
    [MaxLength(512)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The actual API key value (hashed for security)
    /// </summary>
    [MaxLength(256)]
    public string KeyHash { get; set; } = string.Empty;

    /// <summary>
    /// Prefix of the API key for identification (first 8 characters)
    /// </summary>
    [MaxLength(12)]
    public string KeyPrefix { get; set; } = string.Empty;

    /// <summary>
    /// User ID who owns this API key
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Current status of the API key
    /// </summary>
    public ApiKeyStatus Status { get; set; } = ApiKeyStatus.Active;

    /// <summary>
    /// Scopes granted to this API key
    /// </summary>
    public List<string> Scopes { get; set; } = [];

    /// <summary>
    /// Permissions granted to this API key
    /// </summary>
    public List<string> Permissions { get; set; } = [];

    /// <summary>
    /// Rate limiting configuration for this API key
    /// </summary>
    public ApiKeyRateLimit? RateLimit { get; set; }

    /// <summary>
    /// Expiration date of the API key (null for no expiration)
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>
    /// Date when the API key was revoked (if applicable)
    /// </summary>
    public DateTimeOffset? RevokedAt { get; set; }

    /// <summary>
    /// Reason for revocation
    /// </summary>
    [MaxLength(256)]
    public string? RevocationReason { get; set; }

    /// <summary>
    /// User who revoked the API key
    /// </summary>
    public Guid? RevokedBy { get; set; }

    /// <summary>
    /// Last time this API key was used
    /// </summary>
    public DateTimeOffset? LastUsedAt { get; set; }

    /// <summary>
    /// IP address of the last request using this key
    /// </summary>
    [MaxLength(45)] // IPv6 max length
    public string? LastUsedIpAddress { get; set; }

    /// <summary>
    /// User agent of the last request using this key
    /// </summary>
    [MaxLength(512)]
    public string? LastUsedUserAgent { get; set; }

    /// <summary>
    /// Total number of requests made with this API key
    /// </summary>
    public long RequestCount { get; set; } = 0;

    /// <summary>
    /// Number of successful requests (2xx status codes)
    /// </summary>
    public long SuccessfulRequestCount { get; set; } = 0;

    /// <summary>
    /// Number of failed requests (4xx/5xx status codes)
    /// </summary>
    public long FailedRequestCount { get; set; } = 0;

    /// <summary>
    /// Additional metadata as JSON
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = [];

    /// <summary>
    /// IP address restrictions (if any)
    /// </summary>
    public List<string> AllowedIpAddresses { get; set; } = [];

    /// <summary>
    /// Referrer restrictions (if any)
    /// </summary>
    public List<string> AllowedReferrers { get; set; } = [];

    /// <summary>
    /// Environment where this key can be used (dev, staging, prod, etc.)
    /// </summary>
    [MaxLength(32)]
    public string? Environment { get; set; }

    /// <summary>
    /// Tags for categorization and filtering
    /// </summary>
    public List<string> Tags { get; set; } = [];

    /// <summary>
    /// Navigation property to the owning user
    /// </summary>
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Navigation property to the user who revoked the key
    /// </summary>
    public ApplicationUser? RevokedByUser { get; set; }

    private ApiKey() { } // For EF Core

    [SetsRequiredMembers]
    public ApiKey(
        string name,
        string description,
        Guid userId,
        IEnumerable<string> scopes,
        DateTimeOffset? expiresAt = null) {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("API key name cannot be null or empty", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("API key description cannot be null or empty", nameof(description));

        Name = name;
        Description = description;
        UserId = userId;
        Scopes = scopes.ToList();
        ExpiresAt = expiresAt;
        Status = ApiKeyStatus.Active;

        AuditTrail.Add(new AuditEntry {
            Action = "API Key Created",
            UserId = userId,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Sets the API key hash and generates a prefix
    /// </summary>
    /// <param name="keyHash">Hashed API key value</param>
    /// <param name="keyPrefix">First few characters of the original key for identification</param>
    public void SetKeyHash(string keyHash, string keyPrefix) {
        if (string.IsNullOrWhiteSpace(keyHash))
            throw new ArgumentException("Key hash cannot be null or empty", nameof(keyHash));
        if (string.IsNullOrWhiteSpace(keyPrefix))
            throw new ArgumentException("Key prefix cannot be null or empty", nameof(keyPrefix));

        KeyHash = keyHash;
        KeyPrefix = keyPrefix;
    }

    /// <summary>
    /// Updates the API key's scopes
    /// </summary>
    /// <param name="newScopes">New scopes to assign</param>
    /// <param name="updatedBy">User making the change</param>
    public void UpdateScopes(IEnumerable<string> newScopes, Guid updatedBy) {
        var scopesList = newScopes.ToList();
        if (!Scopes.SequenceEqual(scopesList)) {
            Scopes = scopesList;

            AuditTrail.Add(new AuditEntry {
                Action = "API Key Scopes Updated",
                UserId = updatedBy,
                DateTime = DateTimeOffset.UtcNow
            });
        }
    }

    /// <summary>
    /// Updates the API key's permissions
    /// </summary>
    /// <param name="newPermissions">New permissions to assign</param>
    /// <param name="updatedBy">User making the change</param>
    public void UpdatePermissions(IEnumerable<string> newPermissions, Guid updatedBy) {
        var permissionsList = newPermissions.ToList();
        if (!Permissions.SequenceEqual(permissionsList)) {
            Permissions = permissionsList;

            AuditTrail.Add(new AuditEntry {
                Action = "API Key Permissions Updated",
                UserId = updatedBy,
                DateTime = DateTimeOffset.UtcNow
            });
        }
    }

    /// <summary>
    /// Sets rate limiting configuration for the API key
    /// </summary>
    /// <param name="rateLimit">Rate limiting configuration</param>
    /// <param name="updatedBy">User making the change</param>
    public void SetRateLimit(ApiKeyRateLimit rateLimit, Guid updatedBy) {
        RateLimit = rateLimit;

        AuditTrail.Add(new AuditEntry {
            Action = "API Key Rate Limit Updated",
            UserId = updatedBy,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Records usage of the API key
    /// </summary>
    /// <param name="ipAddress">IP address of the request</param>
    /// <param name="userAgent">User agent of the request</param>
    /// <param name="isSuccessful">Whether the request was successful</param>
    public void RecordUsage(string? ipAddress = null, string? userAgent = null, bool isSuccessful = true) {
        LastUsedAt = DateTimeOffset.UtcNow;
        LastUsedIpAddress = ipAddress;
        LastUsedUserAgent = userAgent;
        RequestCount++;

        if (isSuccessful) {
            SuccessfulRequestCount++;
        }
        else {
            FailedRequestCount++;
        }
    }

    /// <summary>
    /// Revokes the API key
    /// </summary>
    /// <param name="reason">Reason for revocation</param>
    /// <param name="revokedBy">User who revoked the key</param>
    public void Revoke(string reason, Guid revokedBy) {
        if (Status == ApiKeyStatus.Revoked)
            return;

        Status = ApiKeyStatus.Revoked;
        RevokedAt = DateTimeOffset.UtcNow;
        RevocationReason = reason;
        RevokedBy = revokedBy;

        AuditTrail.Add(new AuditEntry {
            Action = "API Key Revoked",
            UserId = revokedBy,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Activates a previously inactive API key
    /// </summary>
    /// <param name="activatedBy">User who activated the key</param>
    public void Activate(Guid activatedBy) {
        if (Status == ApiKeyStatus.Active)
            return;

        if (Status == ApiKeyStatus.Revoked)
            throw new InvalidOperationException("Cannot activate a revoked API key");

        Status = ApiKeyStatus.Active;

        AuditTrail.Add(new AuditEntry {
            Action = "API Key Activated",
            UserId = activatedBy,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Deactivates the API key (temporarily disables it)
    /// </summary>
    /// <param name="reason">Reason for deactivation</param>
    /// <param name="deactivatedBy">User who deactivated the key</param>
    public void Deactivate(string reason, Guid deactivatedBy) {
        if (Status == ApiKeyStatus.Inactive)
            return;

        if (Status == ApiKeyStatus.Revoked)
            throw new InvalidOperationException("Cannot deactivate a revoked API key");

        Status = ApiKeyStatus.Inactive;

        AddMetadata("DeactivationReason", reason);

        AuditTrail.Add(new AuditEntry {
            Action = "API Key Deactivated",
            UserId = deactivatedBy,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Adds metadata to the API key
    /// </summary>
    /// <param name="key">Metadata key</param>
    /// <param name="value">Metadata value</param>
    public void AddMetadata(string key, string value) {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Metadata key cannot be null or empty", nameof(key));

        Metadata[key] = value;
    }

    /// <summary>
    /// Adds a tag to the API key
    /// </summary>
    /// <param name="tag">Tag to add</param>
    public void AddTag(string tag) {
        if (string.IsNullOrWhiteSpace(tag))
            throw new ArgumentException("Tag cannot be null or empty", nameof(tag));

        if (!Tags.Contains(tag, StringComparer.OrdinalIgnoreCase)) {
            Tags.Add(tag);
        }
    }

    /// <summary>
    /// Removes a tag from the API key
    /// </summary>
    /// <param name="tag">Tag to remove</param>
    public void RemoveTag(string tag) => Tags.RemoveAll(t => string.Equals(t, tag, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Sets IP address restrictions for the API key
    /// </summary>
    /// <param name="allowedIpAddresses">List of allowed IP addresses or CIDR ranges</param>
    /// <param name="updatedBy">User making the change</param>
    public void SetIpRestrictions(IEnumerable<string> allowedIpAddresses, Guid updatedBy) {
        AllowedIpAddresses = allowedIpAddresses.ToList();

        AuditTrail.Add(new AuditEntry {
            Action = "API Key IP Restrictions Updated",
            UserId = updatedBy,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Sets referrer restrictions for the API key
    /// </summary>
    /// <param name="allowedReferrers">List of allowed referrer URLs</param>
    /// <param name="updatedBy">User making the change</param>
    public void SetReferrerRestrictions(IEnumerable<string> allowedReferrers, Guid updatedBy) {
        AllowedReferrers = allowedReferrers.ToList();

        AuditTrail.Add(new AuditEntry {
            Action = "API Key Referrer Restrictions Updated",
            UserId = updatedBy,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Checks if the API key is currently valid and can be used
    /// </summary>
    /// <returns>True if the key is valid</returns>
    public bool IsValid() {
        if (Status != ApiKeyStatus.Active)
            return false;

        if (ExpiresAt.HasValue && ExpiresAt.Value <= DateTimeOffset.UtcNow)
            return false;

        return true;
    }

    /// <summary>
    /// Checks if the API key has the specified scope
    /// </summary>
    /// <param name="scope">Scope to check</param>
    /// <returns>True if the key has the scope</returns>
    public bool HasScope(string scope) => Scopes.Contains(scope, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Checks if the API key has the specified permission
    /// </summary>
    /// <param name="permission">Permission to check</param>
    /// <returns>True if the key has the permission</returns>
    public bool HasPermission(string permission) => Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Checks if an IP address is allowed to use this API key
    /// </summary>
    /// <param name="ipAddress">IP address to check</param>
    /// <returns>True if the IP address is allowed</returns>
    public bool IsIpAddressAllowed(string ipAddress) {
        if (!AllowedIpAddresses.Any())
            return true; // No restrictions

        // Simple implementation - in production, you'd want CIDR range support
        return AllowedIpAddresses.Contains(ipAddress, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the success rate of requests made with this API key
    /// </summary>
    /// <returns>Success rate as a percentage (0-100)</returns>
    public double GetSuccessRate() {
        if (RequestCount == 0)
            return 0.0;

        return (double)SuccessfulRequestCount / RequestCount * 100.0;
    }
}

/// <summary>
/// API key status enumeration
/// </summary>
public enum ApiKeyStatus {
    Active = 1,
    Inactive = 2,
    Expired = 3,
    Revoked = 4
}

/// <summary>
/// Rate limiting configuration for API keys
/// </summary>
public record ApiKeyRateLimit {
    public int RequestsPerMinute { get; init; } = 100;
    public int RequestsPerHour { get; init; } = 1000;
    public int RequestsPerDay { get; init; } = 10000;
    public bool EnableBurstLimit { get; init; } = false;
    public int? BurstLimit { get; init; }
}