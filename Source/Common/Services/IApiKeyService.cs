namespace MCPHub.Common.Services;

/// <summary>
/// API key management service for secure and scoped API access
/// Supports: Basic Keys → Scoped Keys → Rotated Keys → Enterprise Key Management
/// </summary>
public interface IApiKeyService {
    /// <summary>
    /// Creates a new API key with specified scopes and permissions
    /// </summary>
    /// <param name="createRequest">API key creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>API key creation result</returns>
    Task<ApiKeyCreationResult> CreateApiKeyAsync(CreateApiKeyRequest createRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing API key's properties
    /// </summary>
    /// <param name="keyId">API key identifier</param>
    /// <param name="updateRequest">API key update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>API key update result</returns>
    Task<ApiKeyUpdateResult> UpdateApiKeyAsync(Guid keyId, UpdateApiKeyRequest updateRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes an API key permanently
    /// </summary>
    /// <param name="keyId">API key identifier</param>
    /// <param name="reason">Revocation reason</param>
    /// <param name="revokedBy">User who revoked the key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>API key revocation result</returns>
    Task<ApiKeyRevocationResult> RevokeApiKeyAsync(Guid keyId, string reason, Guid? revokedBy = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates an API key and returns its details
    /// </summary>
    /// <param name="apiKey">API key to validate</param>
    /// <param name="requiredScopes">Optional required scopes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>API key validation result</returns>
    Task<ApiKeyValidationResult> ValidateApiKeyAsync(string apiKey, IEnumerable<string>? requiredScopes = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets API key details by ID
    /// </summary>
    /// <param name="keyId">API key identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>API key details or null if not found</returns>
    Task<ApiKeyDetails?> GetApiKeyAsync(Guid keyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all API keys for a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="includeRevoked">Whether to include revoked keys</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of user API keys</returns>
    Task<IReadOnlyList<ApiKeyDetails>> GetUserApiKeysAsync(Guid userId, bool includeRevoked = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rotates an API key (creates new key and revokes old one)
    /// </summary>
    /// <param name="keyId">API key identifier to rotate</param>
    /// <param name="rotationRequest">Rotation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>API key rotation result</returns>
    Task<ApiKeyRotationResult> RotateApiKeyAsync(Guid keyId, ApiKeyRotationRequest rotationRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates API key scopes
    /// </summary>
    /// <param name="keyId">API key identifier</param>
    /// <param name="scopes">New scopes</param>
    /// <param name="updatedBy">User making the update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Scope update result</returns>
    Task<ApiKeyScopeUpdateResult> UpdateApiKeyScopesAsync(Guid keyId, IEnumerable<string> scopes, Guid? updatedBy = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets rate limits for an API key
    /// </summary>
    /// <param name="keyId">API key identifier</param>
    /// <param name="rateLimits">Rate limit configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Rate limit update result</returns>
    Task<ApiKeyRateLimitResult> SetApiKeyRateLimitsAsync(Guid keyId, ApiKeyRateLimits rateLimits, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets API key usage statistics
    /// </summary>
    /// <param name="keyId">API key identifier</param>
    /// <param name="timeRange">Time range for statistics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>API key usage statistics</returns>
    Task<ApiKeyUsageStatistics> GetApiKeyUsageAsync(Guid keyId, DateTimeRange timeRange, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets usage statistics for all user API keys
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="timeRange">Time range for statistics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User API key usage statistics</returns>
    Task<UserApiKeyUsageStatistics> GetUserApiKeyUsageAsync(Guid userId, DateTimeRange timeRange, CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries API keys based on criteria
    /// </summary>
    /// <param name="criteria">Query criteria</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>API key query result</returns>
    Task<ApiKeyQueryResult> QueryApiKeysAsync(ApiKeyQueryCriteria criteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Activates or deactivates an API key
    /// </summary>
    /// <param name="keyId">API key identifier</param>
    /// <param name="isActive">Whether the key should be active</param>
    /// <param name="reason">Reason for activation/deactivation</param>
    /// <param name="updatedBy">User making the change</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>API key activation result</returns>
    Task<ApiKeyActivationResult> SetApiKeyActiveAsync(Guid keyId, bool isActive, string reason, Guid? updatedBy = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bulk revokes multiple API keys
    /// </summary>
    /// <param name="keyIds">API key identifiers to revoke</param>
    /// <param name="reason">Revocation reason</param>
    /// <param name="revokedBy">User who revoked the keys</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Bulk revocation result</returns>
    Task<BulkApiKeyRevocationResult> BulkRevokeApiKeysAsync(IEnumerable<Guid> keyIds, string reason, Guid? revokedBy = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets API key audit trail
    /// </summary>
    /// <param name="keyId">API key identifier</param>
    /// <param name="timeRange">Optional time range filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>API key audit trail</returns>
    Task<IReadOnlyList<ApiKeyAuditEntry>> GetApiKeyAuditTrailAsync(Guid keyId, DateTimeRange? timeRange = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Create API key request
/// </summary>
public record CreateApiKeyRequest(
    Guid UserId,
    string Name,
    string Description,
    IEnumerable<string> Scopes,
    DateTimeOffset? ExpiresAt = null,
    ApiKeyRateLimits? RateLimits = null,
    IDictionary<string, string>? Metadata = null,
    bool IsActive = true);

/// <summary>
/// API key creation result
/// </summary>
public record ApiKeyCreationResult(
    Guid KeyId,
    string ApiKey,
    bool Success,
    string? ErrorMessage = null);

/// <summary>
/// Update API key request
/// </summary>
public record UpdateApiKeyRequest(
    string? Name = null,
    string? Description = null,
    DateTimeOffset? ExpiresAt = null,
    IDictionary<string, string>? Metadata = null,
    bool? IsActive = null);

/// <summary>
/// API key update result
/// </summary>
public record ApiKeyUpdateResult(
    bool Success,
    string? ErrorMessage = null);

/// <summary>
/// API key revocation result
/// </summary>
public record ApiKeyRevocationResult(
    bool Success,
    DateTimeOffset? RevokedAt = null,
    string? ErrorMessage = null);

/// <summary>
/// API key validation result
/// </summary>
public record ApiKeyValidationResult(
    bool IsValid,
    Guid? KeyId = null,
    Guid? UserId = null,
    IEnumerable<string>? Scopes = null,
    ApiKeyRateLimits? RateLimits = null,
    string? ErrorMessage = null,
    bool IsExpired = false,
    bool IsRevoked = false,
    bool IsInactive = false);

/// <summary>
/// API key details
/// </summary>
public record ApiKeyDetails(
    Guid KeyId,
    string Name,
    string Description,
    Guid UserId,
    string Username,
    string KeyPrefix,
    IEnumerable<string> Scopes,
    ApiKeyStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    DateTimeOffset? ExpiresAt,
    DateTimeOffset? RevokedAt,
    string? RevocationReason,
    DateTimeOffset? LastUsedAt,
    long RequestCount,
    ApiKeyRateLimits? RateLimits,
    IDictionary<string, string> Metadata);

/// <summary>
/// API key rotation request
/// </summary>
public record ApiKeyRotationRequest(
    string? NewName = null,
    DateTimeOffset? NewExpiresAt = null,
    bool RevokeOld = true,
    string? RotationReason = null);

/// <summary>
/// API key rotation result
/// </summary>
public record ApiKeyRotationResult(
    bool Success,
    Guid? NewKeyId = null,
    string? NewApiKey = null,
    DateTimeOffset? OldKeyRevokedAt = null,
    string? ErrorMessage = null);

/// <summary>
/// API key scope update result
/// </summary>
public record ApiKeyScopeUpdateResult(
    bool Success,
    IEnumerable<string>? UpdatedScopes = null,
    string? ErrorMessage = null);

/// <summary>
/// API key rate limits configuration
/// </summary>
public record ApiKeyRateLimits(
    int RequestsPerMinute,
    int RequestsPerHour,
    int RequestsPerDay,
    bool EnableBurstLimit = false,
    int? BurstLimit = null);

/// <summary>
/// API key rate limit result
/// </summary>
public record ApiKeyRateLimitResult(
    bool Success,
    string? ErrorMessage = null);

/// <summary>
/// API key usage statistics
/// </summary>
public record ApiKeyUsageStatistics(
    Guid KeyId,
    DateTimeRange TimeRange,
    long TotalRequests,
    long SuccessfulRequests,
    long FailedRequests,
    IDictionary<string, long> RequestsByEndpoint,
    IDictionary<string, long> RequestsByDay,
    IDictionary<int, long> ResponseCodeCounts,
    DateTimeOffset? LastUsedAt,
    double AverageRequestsPerDay);

/// <summary>
/// User API key usage statistics
/// </summary>
public record UserApiKeyUsageStatistics(
    Guid UserId,
    DateTimeRange TimeRange,
    int TotalApiKeys,
    int ActiveApiKeys,
    long TotalRequests,
    long SuccessfulRequests,
    long FailedRequests,
    IDictionary<Guid, ApiKeyUsageStatistics> KeyUsageStats);

/// <summary>
/// API key query criteria
/// </summary>
public record ApiKeyQueryCriteria(
    Guid? UserId = null,
    IEnumerable<ApiKeyStatus>? Statuses = null,
    IEnumerable<string>? Scopes = null,
    DateTimeRange? CreatedDateRange = null,
    DateTimeRange? ExpirationDateRange = null,
    string? NameFilter = null,
    int PageSize = 50,
    int PageNumber = 1,
    ApiKeySortBy SortBy = ApiKeySortBy.CreatedAt,
    SortDirection SortDirection = SortDirection.Descending);

/// <summary>
/// API key query result
/// </summary>
public record ApiKeyQueryResult(
    IReadOnlyList<ApiKeyDetails> ApiKeys,
    int TotalCount,
    int PageSize,
    int PageNumber,
    int TotalPages);

/// <summary>
/// API key activation result
/// </summary>
public record ApiKeyActivationResult(
    bool Success,
    bool IsActive,
    string? ErrorMessage = null);

/// <summary>
/// Bulk API key revocation result
/// </summary>
public record BulkApiKeyRevocationResult(
    bool Success,
    int KeysRevoked,
    int TotalKeys,
    IEnumerable<Guid>? FailedKeys = null,
    string? ErrorMessage = null);

/// <summary>
/// API key audit entry
/// </summary>
public record ApiKeyAuditEntry(
    Guid AuditId,
    Guid KeyId,
    ApiKeyAuditAction Action,
    string Description,
    Guid? UserId,
    string? Username,
    DateTimeOffset OccurredAt,
    IDictionary<string, object>? Metadata = null);

/// <summary>
/// API key status enumeration
/// </summary>
public enum ApiKeyStatus {
    Active,
    Inactive,
    Expired,
    Revoked
}

/// <summary>
/// API key sorting options
/// </summary>
public enum ApiKeySortBy {
    CreatedAt,
    UpdatedAt,
    ExpiresAt,
    LastUsedAt,
    Name,
    RequestCount
}

/// <summary>
/// API key audit actions
/// </summary>
public enum ApiKeyAuditAction {
    Created,
    Updated,
    Activated,
    Deactivated,
    Revoked,
    ScopesUpdated,
    RateLimitsUpdated,
    Used,
    RateLimited,
    Rotated
}

/// <summary>
/// Standard API key scopes
/// </summary>
public static class ApiKeyScopes {
    public const string ReadPackages = "packages:read";
    public const string WritePackages = "packages:write";
    public const string DeletePackages = "packages:delete";
    public const string PublishPackages = "packages:publish";
    public const string ReadServers = "servers:read";
    public const string WriteServers = "servers:write";
    public const string DeleteServers = "servers:delete";
    public const string RegisterServers = "servers:register";
    public const string ReadUsers = "users:read";
    public const string WriteUsers = "users:write";
    public const string ReadAnalytics = "analytics:read";
    public const string ReadAudit = "audit:read";
    public const string ManageApiKeys = "apikeys:manage";
    public const string AdminAccess = "admin:full";
}

/// <summary>
/// API key permissions for different resource operations
/// </summary>
public static class ApiKeyPermissions {
    public const string PackageRead = "package:read";
    public const string PackageWrite = "package:write";
    public const string PackageDelete = "package:delete";
    public const string PackagePublish = "package:publish";
    public const string ServerRead = "server:read";
    public const string ServerWrite = "server:write";
    public const string ServerDelete = "server:delete";
    public const string ServerRegister = "server:register";
    public const string UserRead = "user:read";
    public const string UserWrite = "user:write";
    public const string AnalyticsRead = "analytics:read";
    public const string AuditRead = "audit:read";
    public const string SecurityRead = "security:read";
    public const string SecurityWrite = "security:write";
    public const string ComplianceRead = "compliance:read";
    public const string AdminFull = "admin:full";
}

/// <summary>
/// API key validation error types
/// </summary>
public enum ApiKeyValidationError {
    InvalidFormat,
    NotFound,
    Expired,
    Revoked,
    Inactive,
    InsufficientScopes,
    RateLimitExceeded,
    IpAddressBlocked,
    UnknownError
}