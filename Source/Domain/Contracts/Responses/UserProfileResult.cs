using MCPHub.Domain.Entities;

namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// User profile information
/// </summary>
public class UserProfileResult {
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public ApplicationUser? User { get; init; }
}