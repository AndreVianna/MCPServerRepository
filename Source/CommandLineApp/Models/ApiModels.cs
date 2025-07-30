namespace MCPHub.CommandLineApp.Models;

/// <summary>
/// Base API response model
/// </summary>
public class ApiResponse<T> {
    /// <summary>
    /// Indicates if the request was successful
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Response data
    /// </summary>
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    /// <summary>
    /// Error details if the request failed
    /// </summary>
    [JsonPropertyName("errors")]
    public List<string>? Errors { get; set; }
}

/// <summary>
/// Search request parameters
/// </summary>
public class SearchRequest {
    /// <summary>
    /// Search query string
    /// </summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// Categories to filter by
    /// </summary>
    public IEnumerable<string>? Categories { get; set; }

    /// <summary>
    /// Minimum trust tier
    /// </summary>
    public string? TrustTier { get; set; }

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Sort field
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction
    /// </summary>
    public string SortDirection { get; set; } = "Ascending";
}

/// <summary>
/// Search result response
/// </summary>
public class SearchResultResponse {
    /// <summary>
    /// Search results
    /// </summary>
    [JsonPropertyName("packages")]
    public List<PackageSearchResult> Packages { get; set; } = new();

    /// <summary>
    /// Total number of packages found
    /// </summary>
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    [JsonPropertyName("page")]
    public int Page { get; set; }

    /// <summary>
    /// Page size
    /// </summary>
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }

    /// <summary>
    /// Search time in milliseconds
    /// </summary>
    [JsonPropertyName("searchTimeMs")]
    public long SearchTimeMs { get; set; }
}

/// <summary>
/// Package search result item
/// </summary>
public class PackageSearchResult {
    /// <summary>
    /// Package ID
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Package name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Package description
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Latest version
    /// </summary>
    [JsonPropertyName("latestVersion")]
    public string LatestVersion { get; set; } = string.Empty;

    /// <summary>
    /// Publisher name
    /// </summary>
    [JsonPropertyName("publisherName")]
    public string PublisherName { get; set; } = string.Empty;

    /// <summary>
    /// Trust tier
    /// </summary>
    [JsonPropertyName("trustTier")]
    public string TrustTier { get; set; } = string.Empty;

    /// <summary>
    /// Download count
    /// </summary>
    [JsonPropertyName("downloadCount")]
    public long DownloadCount { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Last update date
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Package tags
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Security grade
    /// </summary>
    [JsonPropertyName("securityGrade")]
    public string? SecurityGrade { get; set; }

    /// <summary>
    /// Package status
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Package information response
/// </summary>
public class PackageInfoResponse {
    /// <summary>
    /// Package ID
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Package name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Package description
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Package version
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Publisher information
    /// </summary>
    [JsonPropertyName("publisher")]
    public PublisherInfo Publisher { get; set; } = new();

    /// <summary>
    /// Package manifest
    /// </summary>
    [JsonPropertyName("manifest")]
    public PackageManifest? Manifest { get; set; }

    /// <summary>
    /// Download statistics
    /// </summary>
    [JsonPropertyName("downloadCount")]
    public long DownloadCount { get; set; }

    /// <summary>
    /// Trust tier
    /// </summary>
    [JsonPropertyName("trustTier")]
    public string TrustTier { get; set; } = string.Empty;

    /// <summary>
    /// Security grade
    /// </summary>
    [JsonPropertyName("securityGrade")]
    public string? SecurityGrade { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Last update date
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Package tags
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Package status
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Publisher information
/// </summary>
public class PublisherInfo {
    /// <summary>
    /// Publisher ID
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Publisher name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Publisher email
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Publisher type
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Website URL
    /// </summary>
    [JsonPropertyName("websiteUrl")]
    public string? WebsiteUrl { get; set; }

    /// <summary>
    /// Whether the publisher is verified
    /// </summary>
    [JsonPropertyName("isVerified")]
    public bool IsVerified { get; set; }
}

/// <summary>
/// Package manifest information
/// </summary>
public class PackageManifest {
    /// <summary>
    /// MCP protocol version
    /// </summary>
    [JsonPropertyName("mcpVersion")]
    public string McpVersion { get; set; } = string.Empty;

    /// <summary>
    /// Package name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Package version
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Package description
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Package license
    /// </summary>
    [JsonPropertyName("license")]
    public string License { get; set; } = string.Empty;

    /// <summary>
    /// Repository URL
    /// </summary>
    [JsonPropertyName("repository")]
    public string? Repository { get; set; }

    /// <summary>
    /// Homepage URL
    /// </summary>
    [JsonPropertyName("homepage")]
    public string? Homepage { get; set; }

    /// <summary>
    /// Package capabilities
    /// </summary>
    [JsonPropertyName("capabilities")]
    public CapabilitiesInfo Capabilities { get; set; } = new();

    /// <summary>
    /// Package keywords/tags
    /// </summary>
    [JsonPropertyName("keywords")]
    public List<string> Keywords { get; set; } = new();
}

/// <summary>
/// Package capabilities information
/// </summary>
public class CapabilitiesInfo {
    /// <summary>
    /// Number of tools
    /// </summary>
    [JsonPropertyName("toolCount")]
    public int ToolCount { get; set; }

    /// <summary>
    /// Number of resources
    /// </summary>
    [JsonPropertyName("resourceCount")]
    public int ResourceCount { get; set; }

    /// <summary>
    /// Number of prompts
    /// </summary>
    [JsonPropertyName("promptCount")]
    public int PromptCount { get; set; }

    /// <summary>
    /// Tool names
    /// </summary>
    [JsonPropertyName("tools")]
    public List<string> Tools { get; set; } = new();

    /// <summary>
    /// Resource names
    /// </summary>
    [JsonPropertyName("resources")]
    public List<string> Resources { get; set; } = new();

    /// <summary>
    /// Prompt names
    /// </summary>
    [JsonPropertyName("prompts")]
    public List<string> Prompts { get; set; } = new();
}

/// <summary>
/// Package list response
/// </summary>
public class PackageListResponse {
    /// <summary>
    /// List of packages
    /// </summary>
    [JsonPropertyName("packages")]
    public List<PackageSearchResult> Packages { get; set; } = new();

    /// <summary>
    /// Total count
    /// </summary>
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page
    /// </summary>
    [JsonPropertyName("page")]
    public int Page { get; set; }

    /// <summary>
    /// Page size
    /// </summary>
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    /// <summary>
    /// Total pages
    /// </summary>
    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }
}

/// <summary>
/// Package versions response
/// </summary>
public class PackageVersionsResponse {
    /// <summary>
    /// Package versions
    /// </summary>
    [JsonPropertyName("versions")]
    public List<PackageVersionInfo> Versions { get; set; } = new();
}

/// <summary>
/// Package version information
/// </summary>
public class PackageVersionInfo {
    /// <summary>
    /// Version ID
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Version number
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Release notes
    /// </summary>
    [JsonPropertyName("releaseNotes")]
    public string ReleaseNotes { get; set; } = string.Empty;

    /// <summary>
    /// Whether this is a prerelease
    /// </summary>
    [JsonPropertyName("isPrerelease")]
    public bool IsPrerelease { get; set; }

    /// <summary>
    /// Publication date
    /// </summary>
    [JsonPropertyName("publishedAt")]
    public DateTimeOffset PublishedAt { get; set; }

    /// <summary>
    /// Download count for this version
    /// </summary>
    [JsonPropertyName("downloadCount")]
    public long DownloadCount { get; set; }

    /// <summary>
    /// Version status
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Security summary response
/// </summary>
public class SecuritySummaryResponse {
    /// <summary>
    /// Overall security grade
    /// </summary>
    [JsonPropertyName("grade")]
    public string Grade { get; set; } = string.Empty;

    /// <summary>
    /// Security score
    /// </summary>
    [JsonPropertyName("score")]
    public double Score { get; set; }

    /// <summary>
    /// Vulnerability counts
    /// </summary>
    [JsonPropertyName("vulnerabilities")]
    public VulnerabilityCount Vulnerabilities { get; set; } = new();

    /// <summary>
    /// Last scan date
    /// </summary>
    [JsonPropertyName("lastScanned")]
    public DateTimeOffset? LastScanned { get; set; }

    /// <summary>
    /// Security policy information
    /// </summary>
    [JsonPropertyName("hasSecurityPolicy")]
    public bool HasSecurityPolicy { get; set; }
}

/// <summary>
/// Vulnerability count information
/// </summary>
public class VulnerabilityCount {
    /// <summary>
    /// Critical vulnerabilities
    /// </summary>
    [JsonPropertyName("critical")]
    public int Critical { get; set; }

    /// <summary>
    /// High severity vulnerabilities
    /// </summary>
    [JsonPropertyName("high")]
    public int High { get; set; }

    /// <summary>
    /// Medium severity vulnerabilities
    /// </summary>
    [JsonPropertyName("medium")]
    public int Medium { get; set; }

    /// <summary>
    /// Low severity vulnerabilities
    /// </summary>
    [JsonPropertyName("low")]
    public int Low { get; set; }
}

/// <summary>
/// Trust tier response
/// </summary>
public class TrustTierResponse {
    /// <summary>
    /// Current trust tier
    /// </summary>
    [JsonPropertyName("currentTier")]
    public string CurrentTier { get; set; } = string.Empty;

    /// <summary>
    /// Recommended trust tier
    /// </summary>
    [JsonPropertyName("recommendedTier")]
    public string RecommendedTier { get; set; } = string.Empty;

    /// <summary>
    /// Trust score
    /// </summary>
    [JsonPropertyName("trustScore")]
    public double TrustScore { get; set; }

    /// <summary>
    /// Assessment factors
    /// </summary>
    [JsonPropertyName("factors")]
    public Dictionary<string, object> Factors { get; set; } = new();

    /// <summary>
    /// Last assessment date
    /// </summary>
    [JsonPropertyName("lastAssessed")]
    public DateTimeOffset LastAssessed { get; set; }
}

/// <summary>
/// Package publishing request
/// </summary>
public class PublishPackageRequest {
    /// <summary>
    /// Package manifest content as JSON string
    /// </summary>
    [JsonPropertyName("manifestContent")]
    public string ManifestContent { get; set; } = string.Empty;

    /// <summary>
    /// Package archive as base64 encoded string
    /// </summary>
    [JsonPropertyName("packageArchive")]
    public string? PackageArchive { get; set; }

    /// <summary>
    /// URL to package archive (alternative to PackageArchive)
    /// </summary>
    [JsonPropertyName("packageUrl")]
    public string? PackageUrl { get; set; }

    /// <summary>
    /// Package tags for categorization
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// README content in markdown format
    /// </summary>
    [JsonPropertyName("readmeContent")]
    public string? ReadmeContent { get; set; }

    /// <summary>
    /// Changelog content for this version
    /// </summary>
    [JsonPropertyName("changelogContent")]
    public string? ChangelogContent { get; set; }
}

/// <summary>
/// Package publishing response
/// </summary>
public class PublishPackageResponse {
    /// <summary>
    /// Indicates if the publishing was successful
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// List of validation errors
    /// </summary>
    [JsonPropertyName("errors")]
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// List of warnings
    /// </summary>
    [JsonPropertyName("warnings")]
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Published package information
    /// </summary>
    [JsonPropertyName("package")]
    public PackageInfo? Package { get; set; }

    /// <summary>
    /// Publishing timestamp
    /// </summary>
    [JsonPropertyName("publishedAt")]
    public DateTimeOffset PublishedAt { get; set; }

    /// <summary>
    /// Time taken for publishing in milliseconds
    /// </summary>
    [JsonPropertyName("publishTimeMs")]
    public long PublishTimeMs { get; set; }

    /// <summary>
    /// Storage URL where package was uploaded
    /// </summary>
    [JsonPropertyName("storageUrl")]
    public string? StorageUrl { get; set; }
}

/// <summary>
/// Package information for publish response
/// </summary>
public class PackageInfo {
    /// <summary>
    /// Package ID
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Package name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Package version
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Package status
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Manifest validation request
/// </summary>
public class ValidateManifestRequest {
    /// <summary>
    /// Manifest content to validate
    /// </summary>
    [JsonPropertyName("manifestContent")]
    public string ManifestContent { get; set; } = string.Empty;

    /// <summary>
    /// Whether to include warnings in validation
    /// </summary>
    [JsonPropertyName("includeWarnings")]
    public bool IncludeWarnings { get; set; } = true;
}

/// <summary>
/// Manifest validation response
/// </summary>
public class ValidateManifestResponse {
    /// <summary>
    /// Whether the manifest is valid
    /// </summary>
    [JsonPropertyName("isValid")]
    public bool IsValid { get; set; }

    /// <summary>
    /// Validation errors
    /// </summary>
    [JsonPropertyName("errors")]
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Validation warnings
    /// </summary>
    [JsonPropertyName("warnings")]
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Parsed manifest data if valid
    /// </summary>
    [JsonPropertyName("manifest")]
    public PackageManifest? Manifest { get; set; }
}

/// <summary>
/// Download package request
/// </summary>
public class DownloadPackageRequest {
    /// <summary>
    /// User agent for tracking
    /// </summary>
    [JsonPropertyName("userAgent")]
    public string UserAgent { get; set; } = string.Empty;

    /// <summary>
    /// Download method (CLI, Web, API)
    /// </summary>
    [JsonPropertyName("downloadMethod")]
    public string DownloadMethod { get; set; } = "CLI";

    /// <summary>
    /// Client version
    /// </summary>
    [JsonPropertyName("clientVersion")]
    public string? ClientVersion { get; set; }

    /// <summary>
    /// Additional metadata
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Download package response
/// </summary>
public class DownloadPackageResponse {
    /// <summary>
    /// Whether the download request was successful
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Pre-signed download URL
    /// </summary>
    [JsonPropertyName("downloadUrl")]
    public string? DownloadUrl { get; set; }

    /// <summary>
    /// URL expiration time
    /// </summary>
    [JsonPropertyName("expiresAt")]
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>
    /// Download tracking ID
    /// </summary>
    [JsonPropertyName("downloadId")]
    public Guid? DownloadId { get; set; }

    /// <summary>
    /// Error message if failed
    /// </summary>
    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Package file size in bytes
    /// </summary>
    [JsonPropertyName("fileSizeBytes")]
    public long? FileSizeBytes { get; set; }

    /// <summary>
    /// Package file name
    /// </summary>
    [JsonPropertyName("fileName")]
    public string? FileName { get; set; }

    /// <summary>
    /// Package metadata
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Install package request
/// </summary>
public class InstallPackageRequest {
    /// <summary>
    /// Installation path
    /// </summary>
    [JsonPropertyName("installationPath")]
    public string InstallationPath { get; set; } = string.Empty;

    /// <summary>
    /// Installation options
    /// </summary>
    [JsonPropertyName("installationOptions")]
    public Dictionary<string, object>? InstallationOptions { get; set; }

    /// <summary>
    /// Client version
    /// </summary>
    [JsonPropertyName("clientVersion")]
    public string ClientVersion { get; set; } = string.Empty;

    /// <summary>
    /// Is global installation
    /// </summary>
    [JsonPropertyName("isGlobal")]
    public bool IsGlobal { get; set; }

    /// <summary>
    /// Is development dependency
    /// </summary>
    [JsonPropertyName("isDevelopmentDependency")]
    public bool IsDevelopmentDependency { get; set; }
}

/// <summary>
/// Install package response
/// </summary>
public class InstallPackageResponse {
    /// <summary>
    /// Whether the installation request was successful
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Installation tracking ID
    /// </summary>
    [JsonPropertyName("installationId")]
    public Guid? InstallationId { get; set; }

    /// <summary>
    /// Installation status
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// Error message if failed
    /// </summary>
    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Installation metadata
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Package dependencies response
/// </summary>
public class PackageDependenciesResponse {
    /// <summary>
    /// Package dependencies
    /// </summary>
    [JsonPropertyName("dependencies")]
    public List<PackageDependency> Dependencies { get; set; } = new();

    /// <summary>
    /// Development dependencies
    /// </summary>
    [JsonPropertyName("devDependencies")]
    public List<PackageDependency> DevDependencies { get; set; } = new();

    /// <summary>
    /// Peer dependencies
    /// </summary>
    [JsonPropertyName("peerDependencies")]
    public List<PackageDependency> PeerDependencies { get; set; } = new();

    /// <summary>
    /// Whether dependencies were resolved successfully
    /// </summary>
    [JsonPropertyName("resolved")]
    public bool Resolved { get; set; }

    /// <summary>
    /// Dependency resolution errors
    /// </summary>
    [JsonPropertyName("errors")]
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// Package dependency information
/// </summary>
public class PackageDependency {
    /// <summary>
    /// Dependency package name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Version constraint
    /// </summary>
    [JsonPropertyName("versionConstraint")]
    public string VersionConstraint { get; set; } = string.Empty;

    /// <summary>
    /// Resolved version
    /// </summary>
    [JsonPropertyName("resolvedVersion")]
    public string? ResolvedVersion { get; set; }

    /// <summary>
    /// Whether this dependency is optional
    /// </summary>
    [JsonPropertyName("optional")]
    public bool Optional { get; set; }

    /// <summary>
    /// Trust tier of the dependency
    /// </summary>
    [JsonPropertyName("trustTier")]
    public string? TrustTier { get; set; }

    /// <summary>
    /// Whether the dependency is available
    /// </summary>
    [JsonPropertyName("available")]
    public bool Available { get; set; }

    /// <summary>
    /// Reason if not available
    /// </summary>
    [JsonPropertyName("unavailableReason")]
    public string? UnavailableReason { get; set; }
}