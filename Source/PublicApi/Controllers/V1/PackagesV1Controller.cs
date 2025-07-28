using MCPHub.Domain.Contracts.Requests;
using MCPHub.Domain.Contracts.Services;
using MCPHub.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace MCPHub.PublicApi.Controllers.V1;

/// <summary>
/// Controller for package management operations - API Version 1.0
/// </summary>
/// <param name="packageService">Package service for business operations</param>
/// <param name="packagePublishingService">Package publishing service for publication operations</param>
/// <param name="packageInstallationService">Package installation service for download and installation tracking</param>
/// <param name="securityScanService">Security scanning service for package vulnerability analysis</param>
/// <param name="trustTierCalculationService">Trust tier calculation service for package trust assessment</param>
/// <param name="logger">Logger for request tracking</param>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/packages")]
public class PackagesV1Controller(
    IPackageService packageService, 
    IPackagePublishingService packagePublishingService,
    IPackageInstallationService packageInstallationService,
    ISecurityScanService securityScanService,
    ITrustTierCalculationService trustTierCalculationService,
    ILogger<PackagesV1Controller> logger) : BaseApiV1Controller(logger)
{
    private readonly IPackageService _packageService = packageService ?? throw new ArgumentNullException(nameof(packageService));
    private readonly IPackagePublishingService _packagePublishingService = packagePublishingService ?? throw new ArgumentNullException(nameof(packagePublishingService));
    private readonly IPackageInstallationService _packageInstallationService = packageInstallationService ?? throw new ArgumentNullException(nameof(packageInstallationService));
    private readonly ISecurityScanService _securityScanService = securityScanService ?? throw new ArgumentNullException(nameof(securityScanService));
    private readonly ITrustTierCalculationService _trustTierCalculationService = trustTierCalculationService ?? throw new ArgumentNullException(nameof(trustTierCalculationService));

    /// <summary>
    /// Gets all packages
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all packages</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetAllPackages(CancellationToken cancellationToken)
    {
        try
        {
            Logger.LogInformation("Getting all packages");
            var packages = await _packageService.GetAllPackagesAsync(cancellationToken);
            return CreateSuccessResponse(packages, "Packages retrieved successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while getting all packages");
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }

    /// <summary>
    /// Gets a package by its identifier
    /// </summary>
    /// <param name="id">Package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Package if found, 404 if not found</returns>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetPackageById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            Logger.LogInformation("Getting package with ID: {PackageId}", id);
            var package = await _packageService.GetPackageByIdAsync(id, cancellationToken);
            
            if (package == null)
            {
                Logger.LogWarning("Package with ID {PackageId} not found", id);
                return CreateErrorResponse($"Package with ID {id} not found", 404);
            }

            return CreateSuccessResponse(package, "Package retrieved successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while getting package with ID: {PackageId}", id);
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }

    /// <summary>
    /// Gets a package by its name
    /// </summary>
    /// <param name="name">Package name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Package if found, 404 if not found</returns>
    [HttpGet("by-name/{name}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetPackageByName(string name, CancellationToken cancellationToken)
    {
        try
        {
            Logger.LogInformation("Getting package with name: {PackageName}", name);
            var package = await _packageService.GetPackageByNameAsync(name, cancellationToken);
            
            if (package == null)
            {
                Logger.LogWarning("Package with name {PackageName} not found", name);
                return CreateErrorResponse($"Package with name '{name}' not found", 404);
            }

            return CreateSuccessResponse(package, "Package retrieved successfully");
        }
        catch (ArgumentException ex)
        {
            Logger.LogWarning("Invalid package name provided: {PackageName}. Error: {Error}", name, ex.Message);
            return CreateErrorResponse($"Invalid package name: {ex.Message}", 400);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while getting package with name: {PackageName}", name);
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }

    /// <summary>
    /// Gets packages by publisher
    /// </summary>
    /// <param name="publisherId">Publisher identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of packages for the publisher</returns>
    [HttpGet("by-publisher/{publisherId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetPackagesByPublisher(Guid publisherId, CancellationToken cancellationToken)
    {
        try
        {
            Logger.LogInformation("Getting packages for publisher: {PublisherId}", publisherId);
            var packages = await _packageService.GetPackagesByPublisherAsync(publisherId, cancellationToken);
            return CreateSuccessResponse(packages, "Packages retrieved successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while getting packages for publisher: {PublisherId}", publisherId);
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }

    /// <summary>
    /// Searches packages by query
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="pageSize">Number of results per page (default: 20)</param>
    /// <param name="pageIndex">Page index (default: 0)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching packages</returns>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> SearchPackages(
        [FromQuery] string query, 
        [FromQuery] int pageSize = 20, 
        [FromQuery] int pageIndex = 0, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            Logger.LogInformation("Searching packages with query: {Query}, PageSize: {PageSize}, PageIndex: {PageIndex}", 
                query, pageSize, pageIndex);
            
            var packages = await _packageService.SearchPackagesAsync(query, pageSize, pageIndex, cancellationToken);
            return CreateSuccessResponse(packages, "Search completed successfully");
        }
        catch (ArgumentException ex)
        {
            Logger.LogWarning("Invalid search query provided: {Query}. Error: {Error}", query, ex.Message);
            return CreateErrorResponse($"Invalid search query: {ex.Message}", 400);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while searching packages with query: {Query}", query);
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }

    /// <summary>
    /// Searches packages with advanced filtering, pagination, and sorting
    /// </summary>
    /// <param name="q">Search query</param>
    /// <param name="categories">Comma-separated list of categories to filter by</param>
    /// <param name="trustTier">Minimum trust tier required</param>
    /// <param name="page">Page number (1-based, default: 1)</param>
    /// <param name="pageSize">Number of results per page (default: 20, max: 100)</param>
    /// <param name="sortBy">Field to sort by (name, downloads, rating, created)</param>
    /// <param name="sortDirection">Sort direction (ascending, descending)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search result with paginated packages and metadata</returns>
    [HttpGet("search/advanced")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> SearchPackagesAdvanced(
        [FromQuery] string q,
        [FromQuery] string? categories = null,
        [FromQuery] TrustTier? trustTier = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] SortDirection sortDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Logger.LogInformation("Advanced search: Query={Query}, Categories={Categories}, TrustTier={TrustTier}, Page={Page}, PageSize={PageSize}, SortBy={SortBy}, SortDirection={SortDirection}", 
                q, categories, trustTier, page, pageSize, sortBy, sortDirection);

            var searchRequest = new SearchRequest
            {
                Query = q ?? string.Empty,
                Categories = string.IsNullOrWhiteSpace(categories) 
                    ? null 
                    : categories.Split(',', StringSplitOptions.RemoveEmptyEntries)
                               .Select(c => c.Trim())
                               .Where(c => !string.IsNullOrWhiteSpace(c)),
                MinimumTrustTier = trustTier,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDirection = sortDirection
            };

            var searchResult = await _packageService.SearchPackagesAsync(searchRequest, cancellationToken);
            
            // Add pagination headers
            Response.Headers["X-Total-Count"] = searchResult.TotalCount.ToString();
            Response.Headers["X-Page"] = searchResult.Page.ToString();
            Response.Headers["X-Page-Size"] = searchResult.PageSize.ToString();
            Response.Headers["X-Total-Pages"] = searchResult.TotalPages.ToString();
            Response.Headers["X-Search-Time-Ms"] = searchResult.SearchTimeMs.ToString();

            return CreateSuccessResponse(searchResult, "Advanced search completed successfully");
        }
        catch (ArgumentException ex)
        {
            Logger.LogWarning("Invalid search request. Error: {Error}", ex.Message);
            return CreateErrorResponse($"Invalid search request: {ex.Message}", 400);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred during advanced search with query: {Query}", q);
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }

    /// <summary>
    /// Creates a new package
    /// </summary>
    /// <param name="package">Package to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created package</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(object), 201)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> CreatePackage([FromBody] Package package, CancellationToken cancellationToken)
    {
        try
        {
            Logger.LogInformation("Creating new package: {PackageName}", package?.Name ?? "Unknown");
            var createdPackage = await _packageService.CreatePackageAsync(package!, cancellationToken);
            
            Logger.LogInformation("Package created successfully with ID: {PackageId}", createdPackage.Id);
            return CreatedAtAction(nameof(GetPackageById), new { version = "1.0", id = createdPackage.Id }, 
                CreateSuccessResponse(createdPackage, "Package created successfully"));
        }
        catch (ArgumentNullException ex)
        {
            Logger.LogWarning("Null package provided for creation. Error: {Error}", ex.Message);
            return CreateErrorResponse("Package data is required", 400);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while creating package: {PackageName}", package?.Name ?? "Unknown");
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }

    /// <summary>
    /// Updates an existing package
    /// </summary>
    /// <param name="id">Package identifier</param>
    /// <param name="package">Package data to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated package</returns>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> UpdatePackage(Guid id, [FromBody] Package package, CancellationToken cancellationToken)
    {
        try
        {
            if (package.Id != id)
            {
                Logger.LogWarning("Package ID mismatch. URL ID: {UrlId}, Package ID: {PackageId}", id, package.Id);
                return CreateErrorResponse("Package ID in URL does not match package ID in body", 400);
            }

            Logger.LogInformation("Updating package: {PackageId}", id);
            var updatedPackage = await _packageService.UpdatePackageAsync(package, cancellationToken);
            
            Logger.LogInformation("Package updated successfully: {PackageId}", updatedPackage.Id);
            return CreateSuccessResponse(updatedPackage, "Package updated successfully");
        }
        catch (ArgumentNullException ex)
        {
            Logger.LogWarning("Null package provided for update. Error: {Error}", ex.Message);
            return CreateErrorResponse("Package data is required", 400);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while updating package: {PackageId}", id);
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }

    /// <summary>
    /// Deletes a package by its identifier
    /// </summary>
    /// <param name="id">Package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>204 if deleted, 404 if not found</returns>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> DeletePackage(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            Logger.LogInformation("Deleting package: {PackageId}", id);
            var deleted = await _packageService.DeletePackageAsync(id, cancellationToken);
            
            if (!deleted)
            {
                Logger.LogWarning("Package with ID {PackageId} not found for deletion", id);
                return CreateErrorResponse($"Package with ID {id} not found", 404);
            }

            Logger.LogInformation("Package deleted successfully: {PackageId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while deleting package: {PackageId}", id);
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }

    /// <summary>
    /// Publishes a new MCP package
    /// </summary>
    /// <param name="request">Package publishing request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Publishing result with package details or errors</returns>
    [HttpPost("publish")]
    [Authorize]
    [ProducesResponseType(typeof(object), 201)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 429)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> PublishPackage([FromBody] PublishRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (!request.IsValid())
            {
                Logger.LogWarning("Invalid publish request: either PackageArchive or PackageUrl must be provided");
                return CreateErrorResponse("Either PackageArchive or PackageUrl must be provided", 400);
            }

            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                Logger.LogWarning("Unable to determine current user ID for package publishing");
                return CreateErrorResponse("Authentication required", 401);
            }

            Logger.LogInformation("Publishing new package for user: {UserId}", userId);
            var result = await _packagePublishingService.PublishPackageAsync(request, userId, cancellationToken);

            if (!result.Success)
            {
                Logger.LogWarning("Package publishing failed for user {UserId}. Errors: {Errors}", 
                    userId, string.Join(", ", result.Errors));
                return CreateErrorResponse($"Publishing failed: {string.Join(", ", result.Errors)}", 400);
            }

            Logger.LogInformation("Package published successfully: {PackageId}", result.Package?.Id);
            return CreatedAtAction(nameof(GetPackageById), 
                new { version = "1.0", id = result.Package!.Id }, 
                CreateSuccessResponse(result, "Package published successfully"));
        }
        catch (UnauthorizedAccessException)
        {
            Logger.LogWarning("Unauthorized package publishing attempt");
            return CreateErrorResponse("You do not have permission to publish packages", 403);
        }
        catch (ArgumentException ex)
        {
            Logger.LogWarning("Invalid package publishing request: {Error}", ex.Message);
            return CreateErrorResponse($"Invalid request: {ex.Message}", 400);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while publishing package");
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }

    /// <summary>
    /// Publishes a new version of an existing package
    /// </summary>
    /// <param name="packageName">Name of the existing package</param>
    /// <param name="request">Version publishing request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Publishing result with version details or errors</returns>
    [HttpPut("{packageName}/versions")]
    [Authorize]
    [ProducesResponseType(typeof(object), 201)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 429)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> PublishPackageVersion(
        string packageName, 
        [FromBody] PublishVersionRequest request, 
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(packageName))
            {
                Logger.LogWarning("Package name is required for version publishing");
                return CreateErrorResponse("Package name is required", 400);
            }

            if (!request.IsValid())
            {
                Logger.LogWarning("Invalid publish version request: either PackageArchive or PackageUrl must be provided");
                return CreateErrorResponse("Either PackageArchive or PackageUrl must be provided", 400);
            }

            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                Logger.LogWarning("Unable to determine current user ID for package version publishing");
                return CreateErrorResponse("Authentication required", 401);
            }

            Logger.LogInformation("Publishing new version {Version} for package {PackageName} by user {UserId}", 
                request.Version, packageName, userId);
            
            var result = await _packagePublishingService.PublishPackageVersionAsync(packageName, request, userId, cancellationToken);

            if (!result.Success)
            {
                Logger.LogWarning("Package version publishing failed for package {PackageName} version {Version}. Errors: {Errors}", 
                    packageName, request.Version, string.Join(", ", result.Errors));
                
                // Check if it's a not found error
                if (result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
                {
                    return CreateErrorResponse($"Package '{packageName}' not found", 404);
                }

                return CreateErrorResponse($"Version publishing failed: {string.Join(", ", result.Errors)}", 400);
            }

            Logger.LogInformation("Package version published successfully: {PackageVersionId}", result.PackageVersion?.Id);
            return CreatedAtAction(nameof(GetPackageVersions), 
                new { packageName = packageName }, 
                CreateSuccessResponse(result, "Package version published successfully"));
        }
        catch (UnauthorizedAccessException)
        {
            Logger.LogWarning("Unauthorized package version publishing attempt for package {PackageName}", packageName);
            return CreateErrorResponse("You do not have permission to publish versions for this package", 403);
        }
        catch (ArgumentException ex)
        {
            Logger.LogWarning("Invalid package version publishing request for {PackageName}: {Error}", packageName, ex.Message);
            return CreateErrorResponse($"Invalid request: {ex.Message}", 400);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while publishing version for package: {PackageName}", packageName);
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }

    /// <summary>
    /// Validates a package manifest without publishing
    /// </summary>
    /// <param name="manifestContent">Raw manifest JSON content</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with detailed feedback</returns>
    [HttpPost("validate")]
    [Authorize]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> ValidateManifest([FromBody] string manifestContent, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(manifestContent))
            {
                Logger.LogWarning("Empty manifest content provided for validation");
                return CreateErrorResponse("Manifest content is required", 400);
            }

            Logger.LogInformation("Validating package manifest");
            var result = await _packagePublishingService.ValidateManifestAsync(manifestContent, cancellationToken);

            var message = result.Success ? "Manifest validation passed" : "Manifest validation failed";
            Logger.LogInformation("Manifest validation completed. Success: {Success}, Errors: {ErrorCount}, Warnings: {WarningCount}", 
                result.Success, result.Errors.Count, result.Warnings.Count);

            return CreateSuccessResponse(result, message);
        }
        catch (ArgumentException ex)
        {
            Logger.LogWarning("Invalid manifest validation request: {Error}", ex.Message);
            return CreateErrorResponse($"Invalid manifest: {ex.Message}", 400);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while validating manifest");
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }

    /// <summary>
    /// Gets all versions of a specific package
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="includePrerelease">Whether to include prerelease versions</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of package versions</returns>
    [HttpGet("{packageName}/versions")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetPackageVersions(
        string packageName, 
        [FromQuery] bool includePrerelease = false, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(packageName))
            {
                Logger.LogWarning("Package name is required for version listing");
                return CreateErrorResponse("Package name is required", 400);
            }

            Logger.LogInformation("Getting versions for package: {PackageName}, IncludePrerelease: {IncludePrerelease}", 
                packageName, includePrerelease);
            
            var versions = await _packagePublishingService.GetPackageVersionsAsync(packageName, includePrerelease, cancellationToken);

            if (!versions.Any())
            {
                Logger.LogWarning("No versions found for package: {PackageName}", packageName);
                return CreateErrorResponse($"Package '{packageName}' not found or has no versions", 404);
            }

            Logger.LogInformation("Found {VersionCount} versions for package: {PackageName}", versions.Count(), packageName);
            return CreateSuccessResponse(versions, "Package versions retrieved successfully");
        }
        catch (ArgumentException ex)
        {
            Logger.LogWarning("Invalid package name for version listing: {PackageName}. Error: {Error}", packageName, ex.Message);
            return CreateErrorResponse($"Invalid package name: {ex.Message}", 400);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while getting versions for package: {PackageName}", packageName);
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }

    /// <summary>
    /// Checks if a package name is available for publishing
    /// </summary>
    /// <param name="packageName">Package name to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Availability status</returns>
    [HttpGet("check-availability/{packageName}")]
    [Authorize]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> CheckPackageNameAvailability(string packageName, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(packageName))
            {
                Logger.LogWarning("Package name is required for availability check");
                return CreateErrorResponse("Package name is required", 400);
            }

            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                Logger.LogWarning("Unable to determine current user ID for package name availability check");
                return CreateErrorResponse("Authentication required", 401);
            }

            Logger.LogInformation("Checking availability for package name: {PackageName}", packageName);
            var isAvailable = await _packagePublishingService.IsPackageNameAvailableAsync(packageName, userId, cancellationToken);

            var result = new { packageName, isAvailable, checkedAt = DateTimeOffset.UtcNow };
            var message = isAvailable ? "Package name is available" : "Package name is not available";

            Logger.LogInformation("Package name availability check completed. PackageName: {PackageName}, Available: {Available}", 
                packageName, isAvailable);

            return CreateSuccessResponse(result, message);
        }
        catch (ArgumentException ex)
        {
            Logger.LogWarning("Invalid package name for availability check: {PackageName}. Error: {Error}", packageName, ex.Message);
            return CreateErrorResponse($"Invalid package name: {ex.Message}", 400);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while checking package name availability: {PackageName}", packageName);
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }

    /// <summary>
    /// Records a package download and generates a secure download URL
    /// </summary>
    /// <param name="packageName">Name of the package to download</param>
    /// <param name="version">Version of the package to download</param>
    /// <param name="request">Download request details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Download result with secure URL</returns>
    [HttpPost("{packageName}/versions/{version}/download")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 429)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> RecordDownload(
        string packageName,
        string version,
        [FromBody] DownloadRequest request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Package download recording endpoint logic will be implemented when first consumer requires it");
    }

    /// <summary>
    /// Records a package installation
    /// </summary>
    /// <param name="packageName">Name of the package to install</param>
    /// <param name="version">Version of the package to install</param>
    /// <param name="request">Installation request details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Installation result with tracking ID</returns>
    [HttpPost("{packageName}/versions/{version}/install")]
    [Authorize]
    [ProducesResponseType(typeof(object), 201)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> RecordInstallation(
        string packageName,
        string version,
        [FromBody] InstallationRequest request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Package installation recording endpoint logic will be implemented when first consumer requires it");
    }

    /// <summary>
    /// Updates the status of an existing installation
    /// </summary>
    /// <param name="installationId">Installation tracking ID</param>
    /// <param name="status">New installation status</param>
    /// <param name="errorMessage">Error message if status is Failed</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated installation result</returns>
    [HttpPut("installations/{installationId:guid}/status")]
    [Authorize]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> UpdateInstallationStatus(
        Guid installationId,
        [FromBody] object statusUpdate,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Installation status update endpoint logic will be implemented when first consumer requires it");
    }

    /// <summary>
    /// Gets all installations for the current user
    /// </summary>
    /// <param name="includeUninstalled">Whether to include uninstalled packages</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of user installations</returns>
    [HttpGet("installations")]
    [Authorize]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetUserInstallations(
        [FromQuery] bool includeUninstalled = false,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("User installations retrieval endpoint logic will be implemented when first consumer requires it");
    }

    /// <summary>
    /// Gets download statistics for a specific package
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Package download statistics</returns>
    [HttpGet("{packageName}/stats")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetPackageDownloadStats(
        string packageName,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Package download statistics endpoint logic will be implemented when first consumer requires it");
    }

    /// <summary>
    /// Gets a specific installation by its ID
    /// </summary>
    /// <param name="installationId">Installation identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Installation details</returns>
    [HttpGet("installations/{installationId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetInstallation(
        Guid installationId,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Installation retrieval endpoint logic will be implemented when first consumer requires it");
    }

    /// <summary>
    /// Gets the current user ID from claims
    /// </summary>
    /// <returns>Current user ID or Guid.Empty if not found</returns>
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User?.FindFirst("sub")?.Value ?? User?.FindFirst("userId")?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    /// <summary>
    /// Triggers a security scan for a specific package version
    /// </summary>
    /// <param name="packageName">Name of the package to scan</param>
    /// <param name="version">Version of the package to scan</param>
    /// <param name="request">Scan request with options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Security scan result</returns>
    [HttpPost("{packageName}/versions/{version}/scan")]
    [Authorize]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 429)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> TriggerSecurityScan(
        string packageName,
        string version,
        [FromBody] object scanRequest,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Security scan triggering endpoint logic will be implemented when first consumer requires it");
    }

    /// <summary>
    /// Gets the security report for a specific package version
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="version">Version of the package</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Security scan result and report</returns>
    [HttpGet("{packageName}/versions/{version}/security")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetPackageSecurityReport(
        string packageName,
        string version,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Package security report endpoint logic will be implemented when first consumer requires it");
    }

    /// <summary>
    /// Gets a comprehensive security summary for all versions of a package
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Security summary across all package versions</returns>
    [HttpGet("{packageName}/security/summary")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetPackageSecuritySummary(
        string packageName,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Package security summary endpoint logic will be implemented when first consumer requires it");
    }

    /// <summary>
    /// Gets a list of known security vulnerabilities in the system
    /// </summary>
    /// <param name="severity">Filter by minimum severity level</param>
    /// <param name="packageType">Filter by package type</param>
    /// <param name="pageSize">Number of results per page</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of known vulnerabilities</returns>
    [HttpGet("vulnerabilities")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetKnownVulnerabilities(
        [FromQuery] string? severity = null,
        [FromQuery] string? packageType = null,
        [FromQuery] int pageSize = 20,
        [FromQuery] int pageIndex = 0,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Known vulnerabilities listing endpoint logic will be implemented when first consumer requires it");
    }

    /// <summary>
    /// Gets the results of a specific security scan by its ID
    /// </summary>
    /// <param name="scanId">Security scan identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed security scan results</returns>
    [HttpGet("security/scans/{scanId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetSecurityScanResults(
        Guid scanId,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Security scan results endpoint logic will be implemented when first consumer requires it");
    }

    /// <summary>
    /// Gets the trust tier assessment for a specific package
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Trust tier assessment with detailed factors</returns>
    [HttpGet("{packageName}/trust-tier")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetTrustTierAssessment(
        string packageName,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(packageName))
            {
                Logger.LogWarning("Package name is required for trust tier assessment");
                return CreateErrorResponse("Package name is required", 400);
            }

            Logger.LogInformation("Getting trust tier assessment for package: {PackageName}", packageName);
            
            var package = await _packageService.GetPackageByNameAsync(packageName, cancellationToken);
            if (package == null)
            {
                Logger.LogWarning("Package with name {PackageName} not found", packageName);
                return CreateErrorResponse($"Package with name '{packageName}' not found", 404);
            }

            var assessment = await _trustTierCalculationService.GetTrustTierAssessmentAsync(package.Id, cancellationToken);
            
            Logger.LogInformation("Trust tier assessment completed for package: {PackageName}, CurrentTier: {CurrentTier}, RecommendedTier: {RecommendedTier}", 
                packageName, assessment.CurrentTier, assessment.RecommendedTier);

            return CreateSuccessResponse(assessment, "Trust tier assessment retrieved successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while getting trust tier assessment for package: {PackageName}", packageName);
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }

    /// <summary>
    /// Triggers a trust tier recalculation for a specific package
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Recalculated trust tier result</returns>
    [HttpPost("{packageName}/trust-tier/recalculate")]
    [Authorize]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> RecalculateTrustTier(
        string packageName,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(packageName))
            {
                Logger.LogWarning("Package name is required for trust tier recalculation");
                return CreateErrorResponse("Package name is required", 400);
            }

            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                Logger.LogWarning("Unable to determine current user ID for trust tier recalculation");
                return CreateErrorResponse("Authentication required", 401);
            }

            Logger.LogInformation("Recalculating trust tier for package: {PackageName} by user: {UserId}", packageName, userId);
            
            var package = await _packageService.GetPackageByNameAsync(packageName, cancellationToken);
            if (package == null)
            {
                Logger.LogWarning("Package with name {PackageName} not found", packageName);
                return CreateErrorResponse($"Package with name '{packageName}' not found", 404);
            }

            var newTier = await _trustTierCalculationService.CalculatePackageTrustTierAsync(package.Id, cancellationToken);
            var assessment = await _trustTierCalculationService.GetTrustTierAssessmentAsync(package.Id, cancellationToken);
            
            var result = new
            {
                packageName,
                packageId = package.Id,
                previousTier = package.TrustTier,
                newTier,
                assessment,
                recalculatedAt = DateTimeOffset.UtcNow,
                recalculatedBy = userId
            };

            Logger.LogInformation("Trust tier recalculation completed for package: {PackageName}, PreviousTier: {PreviousTier}, NewTier: {NewTier}", 
                packageName, package.TrustTier, newTier);

            return CreateSuccessResponse(result, "Trust tier recalculation completed successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while recalculating trust tier for package: {PackageName}", packageName);
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }

    /// <summary>
    /// Gets the trust tier change history for a specific package
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="limit">Maximum number of history entries to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Chronological history of trust tier changes</returns>
    [HttpGet("{packageName}/trust-tier/history")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetTrustTierHistory(
        string packageName,
        [FromQuery] int limit = 50,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(packageName))
            {
                Logger.LogWarning("Package name is required for trust tier history");
                return CreateErrorResponse("Package name is required", 400);
            }

            if (limit <= 0 || limit > 200)
            {
                Logger.LogWarning("Invalid limit for trust tier history: {Limit}", limit);
                return CreateErrorResponse("Limit must be between 1 and 200", 400);
            }

            Logger.LogInformation("Getting trust tier history for package: {PackageName}, Limit: {Limit}", packageName, limit);
            
            var package = await _packageService.GetPackageByNameAsync(packageName, cancellationToken);
            if (package == null)
            {
                Logger.LogWarning("Package with name {PackageName} not found", packageName);
                return CreateErrorResponse($"Package with name '{packageName}' not found", 404);
            }

            var history = await _trustTierCalculationService.GetTrustTierHistoryAsync(package.Id, limit, cancellationToken);
            
            Logger.LogInformation("Found {HistoryCount} trust tier history entries for package: {PackageName}", 
                history.Count(), packageName);

            return CreateSuccessResponse(history, "Trust tier history retrieved successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while getting trust tier history for package: {PackageName}", packageName);
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }

    /// <summary>
    /// Gets platform-wide trust tier statistics
    /// </summary>
    /// <param name="periodDays">Number of days to include in recent statistics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Platform trust tier statistics</returns>
    [HttpGet("trust-tiers/statistics")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetTrustTierStatistics(
        [FromQuery] int periodDays = 30,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (periodDays <= 0 || periodDays > 365)
            {
                Logger.LogWarning("Invalid period days for trust tier statistics: {PeriodDays}", periodDays);
                return CreateErrorResponse("Period days must be between 1 and 365", 400);
            }

            Logger.LogInformation("Getting trust tier statistics for period: {PeriodDays} days", periodDays);
            
            var statistics = await _trustTierCalculationService.GetTrustTierStatisticsAsync(periodDays, cancellationToken);
            
            Logger.LogInformation("Trust tier statistics retrieved successfully for {TotalPackages} packages", 
                statistics.TotalPackages);

            return CreateSuccessResponse(statistics, "Trust tier statistics retrieved successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while getting trust tier statistics");
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }

    /// <summary>
    /// Manually adjusts the trust tier for a package (admin only)
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="request">Trust tier adjustment request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Trust tier adjustment result</returns>
    [HttpPut("{packageName}/trust-tier")]
    [Authorize(Roles = "Administrator")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 403)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> AdjustTrustTier(
        string packageName,
        [FromBody] object adjustmentRequest,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Manual trust tier adjustment endpoint logic will be implemented when first consumer requires it");
    }

    /// <summary>
    /// Validates whether a package meets the requirements for a specific trust tier
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="targetTier">Target trust tier to validate against</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Trust tier validation result</returns>
    [HttpGet("{packageName}/trust-tier/validate/{targetTier}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> ValidateTrustTierEligibility(
        string packageName,
        TrustTier targetTier,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(packageName))
            {
                Logger.LogWarning("Package name is required for trust tier validation");
                return CreateErrorResponse("Package name is required", 400);
            }

            Logger.LogInformation("Validating trust tier eligibility for package: {PackageName}, TargetTier: {TargetTier}", 
                packageName, targetTier);
            
            var package = await _packageService.GetPackageByNameAsync(packageName, cancellationToken);
            if (package == null)
            {
                Logger.LogWarning("Package with name {PackageName} not found", packageName);
                return CreateErrorResponse($"Package with name '{packageName}' not found", 404);
            }

            var validationResult = await _trustTierCalculationService.ValidateTrustTierEligibilityAsync(
                package.Id, targetTier, cancellationToken);
            
            Logger.LogInformation("Trust tier validation completed for package: {PackageName}, TargetTier: {TargetTier}, IsEligible: {IsEligible}", 
                packageName, targetTier, validationResult.IsEligible);

            return CreateSuccessResponse(validationResult, "Trust tier validation completed successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while validating trust tier eligibility for package: {PackageName}", packageName);
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }

    /// <summary>
    /// Gets the client IP address from the request
    /// </summary>
    /// <returns>Client IP address</returns>
    private string GetClientIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}