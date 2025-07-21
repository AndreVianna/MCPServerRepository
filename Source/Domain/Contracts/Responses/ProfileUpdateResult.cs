using MCPHub.Domain.Entities;

namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// Result of profile update operation
/// </summary>
public class ProfileUpdateResult {
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public ApplicationUser? User { get; init; }
}