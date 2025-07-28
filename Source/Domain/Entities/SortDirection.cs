namespace MCPHub.Domain.Entities;

/// <summary>
/// Defines the direction for sorting operations
/// </summary>
public enum SortDirection {
    /// <summary>
    /// Sort in ascending order (A-Z, 0-9, oldest to newest)
    /// </summary>
    Ascending,

    /// <summary>
    /// Sort in descending order (Z-A, 9-0, newest to oldest)
    /// </summary>
    Descending
}