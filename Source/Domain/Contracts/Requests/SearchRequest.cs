using MCPHub.Domain.Entities;

namespace MCPHub.Domain.Contracts.Requests;

/// <summary>
/// Request for searching packages with advanced filtering and pagination
/// </summary>
public class SearchRequest {
    /// <summary>
    /// Search query term for full-text search across package name, description, and tags
    /// </summary>
    [Required]
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// Optional categories to filter packages by
    /// </summary>
    public IEnumerable<string>? Categories { get; set; }

    /// <summary>
    /// Minimum trust tier required for packages in results
    /// </summary>
    public TrustTier? MinimumTrustTier { get; set; }

    /// <summary>
    /// Page number for pagination (1-based)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page must be 1 or greater")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of results per page
    /// </summary>
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Field to sort results by
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Direction to sort results (ascending or descending)
    /// </summary>
    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;

    /// <summary>
    /// Validates the search request
    /// </summary>
    /// <returns>Validation result</returns>
    public bool IsValid(out string? errorMessage) {
        errorMessage = null;

        if (string.IsNullOrWhiteSpace(Query)) {
            errorMessage = "Query is required";
            return false;
        }

        if (Page < 1) {
            errorMessage = "Page must be 1 or greater";
            return false;
        }

        if (PageSize < 1 || PageSize > 100) {
            errorMessage = "Page size must be between 1 and 100";
            return false;
        }

        return true;
    }
}