using MCPHub.Domain.Entities;

namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// Result of registration operation
/// </summary>
public class RegistrationResult {
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public ApplicationUser? User { get; init; }
    public IEnumerable<string>? ValidationErrors { get; init; }
}