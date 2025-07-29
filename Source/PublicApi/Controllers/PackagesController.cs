using MCPHub.Domain.Contracts.Requests;
using MCPHub.Domain.Contracts.Services;
using MCPHub.Domain.Entities;

namespace MCPHub.PublicApi.Controllers;

/// <summary>
/// Controller for package management operations
/// </summary>
/// <param name="packageService">Package service for business operations</param>
/// <param name="logger">Logger for request tracking</param>
[ApiController]
[Route("api/[controller]")]
public class PackagesController(IPackageService packageService, ILogger<PackagesController> logger) : ControllerBase {
    private readonly IPackageService _packageService = packageService ?? throw new ArgumentNullException(nameof(packageService));
    private readonly ILogger<PackagesController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Gets all packages
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all packages</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllPackages(CancellationToken cancellationToken) {
        try {
            _logger.LogInformation("Getting all packages");
            var packages = await _packageService.GetAllPackagesAsync(cancellationToken);
            return Ok(packages);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error occurred while getting all packages");
            return StatusCode(500, "An error occurred while processing your request");
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
    public async Task<IActionResult> GetPackageById(Guid id, CancellationToken cancellationToken) {
        try {
            _logger.LogInformation("Getting package with ID: {PackageId}", id);
            var package = await _packageService.GetPackageByIdAsync(id, cancellationToken);

            if (package == null) {
                _logger.LogWarning("Package with ID {PackageId} not found", id);
                return NotFound($"Package with ID {id} not found");
            }

            return Ok(package);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error occurred while getting package with ID: {PackageId}", id);
            return StatusCode(500, "An error occurred while processing your request");
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
    public async Task<IActionResult> GetPackageByName(string name, CancellationToken cancellationToken) {
        try {
            _logger.LogInformation("Getting package with name: {PackageName}", name);
            var package = await _packageService.GetPackageByNameAsync(name, cancellationToken);

            if (package == null) {
                _logger.LogWarning("Package with name {PackageName} not found", name);
                return NotFound($"Package with name '{name}' not found");
            }

            return Ok(package);
        }
        catch (ArgumentException ex) {
            _logger.LogWarning("Invalid package name provided: {PackageName}. Error: {Error}", name, ex.Message);
            return BadRequest($"Invalid package name: {ex.Message}");
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error occurred while getting package with name: {PackageName}", name);
            return StatusCode(500, "An error occurred while processing your request");
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
    public async Task<IActionResult> GetPackagesByPublisher(Guid publisherId, CancellationToken cancellationToken) {
        try {
            _logger.LogInformation("Getting packages for publisher: {PublisherId}", publisherId);
            var packages = await _packageService.GetPackagesByPublisherAsync(publisherId, cancellationToken);
            return Ok(packages);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error occurred while getting packages for publisher: {PublisherId}", publisherId);
            return StatusCode(500, "An error occurred while processing your request");
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
    public async Task<IActionResult> SearchPackages(
        [FromQuery] string query,
        [FromQuery] int pageSize = 20,
        [FromQuery] int pageIndex = 0,
        CancellationToken cancellationToken = default) {
        try {
            _logger.LogInformation("Searching packages with query: {Query}, PageSize: {PageSize}, PageIndex: {PageIndex}",
                query, pageSize, pageIndex);

            var packages = await _packageService.SearchPackagesAsync(query, pageSize, pageIndex, cancellationToken);
            return Ok(packages);
        }
        catch (ArgumentException ex) {
            _logger.LogWarning("Invalid search query provided: {Query}. Error: {Error}", query, ex.Message);
            return BadRequest($"Invalid search query: {ex.Message}");
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error occurred while searching packages with query: {Query}", query);
            return StatusCode(500, "An error occurred while processing your request");
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
    public async Task<IActionResult> SearchPackagesAdvanced(
        [FromQuery] string q,
        [FromQuery] string? categories = null,
        [FromQuery] TrustTier? trustTier = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] SortDirection sortDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default) {
        try {
            _logger.LogInformation("Advanced search: Query={Query}, Categories={Categories}, TrustTier={TrustTier}, Page={Page}, PageSize={PageSize}, SortBy={SortBy}, SortDirection={SortDirection}",
                q, categories, trustTier, page, pageSize, sortBy, sortDirection);

            var searchRequest = new SearchRequest {
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

            return Ok(searchResult);
        }
        catch (ArgumentException ex) {
            _logger.LogWarning("Invalid search request. Error: {Error}", ex.Message);
            return BadRequest($"Invalid search request: {ex.Message}");
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error occurred during advanced search with query: {Query}", q);
            return StatusCode(500, "An error occurred while processing your request");
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
    public async Task<IActionResult> CreatePackage([FromBody] Package package, CancellationToken cancellationToken) {
        try {
            _logger.LogInformation("Creating new package: {PackageName}", package?.Name ?? "Unknown");
            var createdPackage = await _packageService.CreatePackageAsync(package!, cancellationToken);

            _logger.LogInformation("Package created successfully with ID: {PackageId}", createdPackage.Id);
            return CreatedAtAction(nameof(GetPackageById), new { id = createdPackage.Id }, createdPackage);
        }
        catch (ArgumentNullException ex) {
            _logger.LogWarning("Null package provided for creation. Error: {Error}", ex.Message);
            return BadRequest("Package data is required");
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error occurred while creating package: {PackageName}", package?.Name ?? "Unknown");
            return StatusCode(500, "An error occurred while processing your request");
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
    public async Task<IActionResult> UpdatePackage(Guid id, [FromBody] Package package, CancellationToken cancellationToken) {
        try {
            if (package.Id != id) {
                _logger.LogWarning("Package ID mismatch. URL ID: {UrlId}, Package ID: {PackageId}", id, package.Id);
                return BadRequest("Package ID in URL does not match package ID in body");
            }

            _logger.LogInformation("Updating package: {PackageId}", id);
            var updatedPackage = await _packageService.UpdatePackageAsync(package, cancellationToken);

            _logger.LogInformation("Package updated successfully: {PackageId}", updatedPackage.Id);
            return Ok(updatedPackage);
        }
        catch (ArgumentNullException ex) {
            _logger.LogWarning("Null package provided for update. Error: {Error}", ex.Message);
            return BadRequest("Package data is required");
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error occurred while updating package: {PackageId}", id);
            return StatusCode(500, "An error occurred while processing your request");
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
    public async Task<IActionResult> DeletePackage(Guid id, CancellationToken cancellationToken) {
        try {
            _logger.LogInformation("Deleting package: {PackageId}", id);
            var deleted = await _packageService.DeletePackageAsync(id, cancellationToken);

            if (!deleted) {
                _logger.LogWarning("Package with ID {PackageId} not found for deletion", id);
                return NotFound($"Package with ID {id} not found");
            }

            _logger.LogInformation("Package deleted successfully: {PackageId}", id);
            return NoContent();
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error occurred while deleting package: {PackageId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }
}