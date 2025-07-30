namespace MCPHub.Common.Services;

/// <summary>
/// Role-Based Access Control service for granular permissions management
/// Supports: Simple Roles → Hierarchical Roles → Fine-grained Permissions → Enterprise RBAC
/// </summary>
public interface IRoleBasedAccessService {
    /// <summary>
    /// Checks if a user has permission to perform an action on a resource
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="resource">Resource identifier</param>
    /// <param name="action">Action to perform</param>
    /// <param name="context">Optional context information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authorization result</returns>
    Task<AuthorizationResult> CheckPermissionAsync(
        Guid userId,
        string resource,
        string action,
        PermissionContext? context = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks multiple permissions for a user in a single call
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="permissionChecks">List of permission checks to perform</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Bulk authorization result</returns>
    Task<BulkAuthorizationResult> CheckPermissionsAsync(
        Guid userId,
        IEnumerable<PermissionCheck> permissionChecks,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a role to a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="roleId">Role identifier</param>
    /// <param name="assignedBy">User who assigned the role</param>
    /// <param name="expiresAt">Optional expiration date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Role assignment result</returns>
    Task<RoleAssignmentResult> AssignRoleAsync(
        Guid userId,
        Guid roleId,
        Guid? assignedBy = null,
        DateTimeOffset? expiresAt = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a role from a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="roleId">Role identifier</param>
    /// <param name="removedBy">User who removed the role</param>
    /// <param name="reason">Reason for removal</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Role removal result</returns>
    Task<RoleRemovalResult> RemoveRoleAsync(
        Guid userId,
        Guid roleId,
        Guid? removedBy = null,
        string? reason = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new role with specified permissions
    /// </summary>
    /// <param name="createRequest">Role creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Role creation result</returns>
    Task<RoleCreationResult> CreateRoleAsync(CreateRoleRequest createRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing role
    /// </summary>
    /// <param name="roleId">Role identifier</param>
    /// <param name="updateRequest">Role update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Role update result</returns>
    Task<RoleUpdateResult> UpdateRoleAsync(Guid roleId, UpdateRoleRequest updateRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a role
    /// </summary>
    /// <param name="roleId">Role identifier</param>
    /// <param name="deletedBy">User who deleted the role</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Role deletion result</returns>
    Task<RoleDeletionResult> DeleteRoleAsync(Guid roleId, Guid? deletedBy = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles for a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="includeExpired">Whether to include expired roles</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User roles</returns>
    Task<IReadOnlyList<UserRole>> GetUserRolesAsync(Guid userId, bool includeExpired = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all permissions for a user (computed from roles)
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User permissions</returns>
    Task<IReadOnlyList<UserPermission>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets role details by ID
    /// </summary>
    /// <param name="roleId">Role identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Role details or null if not found</returns>
    Task<RoleDetails?> GetRoleAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all available roles
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive roles</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Available roles</returns>
    Task<IReadOnlyList<RoleDetails>> GetAllRolesAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a permission to a role
    /// </summary>
    /// <param name="roleId">Role identifier</param>
    /// <param name="permission">Permission to add</param>
    /// <param name="addedBy">User who added the permission</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Permission addition result</returns>
    Task<PermissionAdditionResult> AddPermissionToRoleAsync(
        Guid roleId,
        RolePermission permission,
        Guid? addedBy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a permission from a role
    /// </summary>
    /// <param name="roleId">Role identifier</param>
    /// <param name="resource">Resource identifier</param>
    /// <param name="action">Action identifier</param>
    /// <param name="removedBy">User who removed the permission</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Permission removal result</returns>
    Task<PermissionRemovalResult> RemovePermissionFromRoleAsync(
        Guid roleId,
        string resource,
        string action,
        Guid? removedBy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users who have a specific role
    /// </summary>
    /// <param name="roleId">Role identifier</param>
    /// <param name="includeExpired">Whether to include users with expired role assignments</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Users with the role</returns>
    Task<IReadOnlyList<UserWithRole>> GetUsersWithRoleAsync(Guid roleId, bool includeExpired = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a permission policy that can be reused across roles
    /// </summary>
    /// <param name="policyRequest">Permission policy request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Permission policy result</returns>
    Task<PermissionPolicyResult> CreatePermissionPolicyAsync(CreatePermissionPolicyRequest policyRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Evaluates permission policies against a request
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="resource">Resource identifier</param>
    /// <param name="action">Action identifier</param>
    /// <param name="context">Permission context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Policy evaluation result</returns>
    Task<PolicyEvaluationResult> EvaluatePermissionPoliciesAsync(
        Guid userId,
        string resource,
        string action,
        PermissionContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets RBAC statistics and metrics
    /// </summary>
    /// <param name="timeRange">Time range for statistics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>RBAC statistics</returns>
    Task<RbacStatistics> GetRbacStatisticsAsync(DateTimeRange timeRange, CancellationToken cancellationToken = default);
}

/// <summary>
/// Authorization result
/// </summary>
public record AuthorizationResult(
    bool IsAuthorized,
    string? DenialReason = null,
    IEnumerable<string>? RequiredPermissions = null,
    IEnumerable<string>? GrantedPermissions = null,
    IDictionary<string, object>? Context = null);

/// <summary>
/// Permission check request
/// </summary>
public record PermissionCheck(
    string Resource,
    string Action,
    PermissionContext? Context = null);

/// <summary>
/// Bulk authorization result
/// </summary>
public record BulkAuthorizationResult(
    IReadOnlyDictionary<PermissionCheck, AuthorizationResult> Results,
    int AuthorizedCount,
    int DeniedCount,
    bool AllAuthorized);

/// <summary>
/// Permission context for authorization decisions
/// </summary>
public record PermissionContext(
    string? ResourceId = null,
    Guid? OrganizationId = null,
    IDictionary<string, object>? Properties = null,
    string? ClientIpAddress = null,
    string? UserAgent = null);

/// <summary>
/// Role assignment result
/// </summary>
public record RoleAssignmentResult(
    bool Success,
    DateTimeOffset? AssignedAt = null,
    string? ErrorMessage = null);

/// <summary>
/// Role removal result
/// </summary>
public record RoleRemovalResult(
    bool Success,
    DateTimeOffset? RemovedAt = null,
    string? ErrorMessage = null);

/// <summary>
/// Create role request
/// </summary>
public record CreateRoleRequest(
    string Name,
    string Description,
    IEnumerable<RolePermission> Permissions,
    RoleType Type = RoleType.Custom,
    Guid? ParentRoleId = null,
    bool IsActive = true,
    IDictionary<string, string>? Metadata = null);

/// <summary>
/// Role creation result
/// </summary>
public record RoleCreationResult(
    Guid RoleId,
    bool Success,
    string? ErrorMessage = null);

/// <summary>
/// Update role request
/// </summary>
public record UpdateRoleRequest(
    string? Name = null,
    string? Description = null,
    IEnumerable<RolePermission>? Permissions = null,
    bool? IsActive = null,
    IDictionary<string, string>? Metadata = null);

/// <summary>
/// Role update result
/// </summary>
public record RoleUpdateResult(
    bool Success,
    string? ErrorMessage = null);

/// <summary>
/// Role deletion result
/// </summary>
public record RoleDeletionResult(
    bool Success,
    int AffectedUsers,
    string? ErrorMessage = null);

/// <summary>
/// User role information
/// </summary>
public record UserRole(
    Guid RoleId,
    string RoleName,
    string Description,
    RoleType Type,
    DateTimeOffset AssignedAt,
    DateTimeOffset? ExpiresAt,
    Guid? AssignedBy,
    string? AssignedByUsername,
    bool IsActive,
    bool IsExpired);

/// <summary>
/// User permission information
/// </summary>
public record UserPermission(
    string Resource,
    string Action,
    PermissionEffect Effect,
    IEnumerable<string> Sources,
    IDictionary<string, object>? Conditions = null);

/// <summary>
/// Role details
/// </summary>
public record RoleDetails(
    Guid RoleId,
    string Name,
    string Description,
    RoleType Type,
    IEnumerable<RolePermission> Permissions,
    Guid? ParentRoleId,
    string? ParentRoleName,
    int UserCount,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    IDictionary<string, string> Metadata);

/// <summary>
/// Role permission definition
/// </summary>
public record RolePermission(
    string Resource,
    string Action,
    PermissionEffect Effect = PermissionEffect.Allow,
    IDictionary<string, object>? Conditions = null);

/// <summary>
/// Permission addition result
/// </summary>
public record PermissionAdditionResult(
    bool Success,
    string? ErrorMessage = null);

/// <summary>
/// Permission removal result
/// </summary>
public record PermissionRemovalResult(
    bool Success,
    string? ErrorMessage = null);

/// <summary>
/// User with role information
/// </summary>
public record UserWithRole(
    Guid UserId,
    string Username,
    string Email,
    DateTimeOffset AssignedAt,
    DateTimeOffset? ExpiresAt,
    bool IsActive,
    bool IsExpired);

/// <summary>
/// Create permission policy request
/// </summary>
public record CreatePermissionPolicyRequest(
    string Name,
    string Description,
    string PolicyDocument,
    PolicyType Type = PolicyType.Json,
    bool IsActive = true);

/// <summary>
/// Permission policy result
/// </summary>
public record PermissionPolicyResult(
    Guid PolicyId,
    bool Success,
    string? ErrorMessage = null);

/// <summary>
/// Policy evaluation result
/// </summary>
public record PolicyEvaluationResult(
    PermissionEffect Effect,
    IEnumerable<string> MatchedPolicies,
    string? Reason = null,
    IDictionary<string, object>? EvaluationContext = null);

/// <summary>
/// RBAC statistics
/// </summary>
public record RbacStatistics(
    DateTimeRange TimeRange,
    int TotalRoles,
    int ActiveRoles,
    int CustomRoles,
    int SystemRoles,
    int TotalUsers,
    int UsersWithRoles,
    int TotalPermissions,
    int AuthorizationChecks,
    int AuthorizedRequests,
    int DeniedRequests,
    double AuthorizationRate,
    IDictionary<string, int> TopDeniedResources,
    IDictionary<string, int> TopUsedPermissions);

/// <summary>
/// Role types
/// </summary>
public enum RoleType {
    System,
    Predefined,
    Custom,
    Temporary,
}

/// <summary>
/// Permission effects
/// </summary>
public enum PermissionEffect {
    Allow,
    Deny,
}

/// <summary>
/// Permission policy types
/// </summary>
public enum PolicyType {
    Json,
    Rego,
    Custom,
}

/// <summary>
/// Standard system roles
/// </summary>
public static class SystemRoles {
    public const string SuperAdmin = "system:super-admin";
    public const string Admin = "system:admin";
    public const string Moderator = "system:moderator";
    public const string User = "system:user";
    public const string Guest = "system:guest";
    public const string Publisher = "system:publisher";
    public const string Reviewer = "system:reviewer";
    public const string AuditViewer = "system:audit-viewer";
    public const string SecurityAnalyst = "system:security-analyst";
}

/// <summary>
/// Standard resource types
/// </summary>
public static class ResourceTypes {
    public const string Package = "package";
    public const string Server = "server";
    public const string User = "user";
    public const string Organization = "organization";
    public const string ApiKey = "apikey";
    public const string Role = "role";
    public const string Permission = "permission";
    public const string SecurityPolicy = "security-policy";
    public const string AuditLog = "audit-log";
    public const string System = "system";
}

/// <summary>
/// Standard actions
/// </summary>
public static class Actions {
    public const string Create = "create";
    public const string Read = "read";
    public const string Update = "update";
    public const string Delete = "delete";
    public const string List = "list";
    public const string Publish = "publish";
    public const string Approve = "approve";
    public const string Reject = "reject";
    public const string Manage = "manage";
    public const string Execute = "execute";
    public const string Download = "download";
    public const string Upload = "upload";
    public const string Share = "share";
    public const string Export = "export";
}

/// <summary>
/// Standard permissions
/// </summary>
public static class StandardPermissions {
    // Package permissions
    public const string PackageRead = "package:read";
    public const string PackageWrite = "package:write";
    public const string PackageDelete = "package:delete";
    public const string PackagePublish = "package:publish";
    public const string PackageApprove = "package:approve";

    // Server permissions
    public const string ServerRead = "server:read";
    public const string ServerWrite = "server:write";
    public const string ServerDelete = "server:delete";
    public const string ServerRegister = "server:register";

    // User permissions
    public const string UserRead = "user:read";
    public const string UserWrite = "user:write";
    public const string UserDelete = "user:delete";
    public const string UserManage = "user:manage";

    // Admin permissions
    public const string AdminFull = "admin:full";
    public const string AdminRead = "admin:read";
    public const string AdminWrite = "admin:write";

    // Security permissions
    public const string SecurityRead = "security:read";
    public const string SecurityWrite = "security:write";
    public const string SecurityManage = "security:manage";

    // Audit permissions
    public const string AuditRead = "audit:read";
    public const string AuditExport = "audit:export";

    // System permissions
    public const string SystemConfig = "system:config";
    public const string SystemMaintenance = "system:maintenance";
}