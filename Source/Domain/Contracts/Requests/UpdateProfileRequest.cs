namespace MCPHub.Domain.Contracts.Requests;

/// <summary>
/// Profile update request
/// </summary>
public class UpdateProfileRequest {
    public string? DisplayName { get; init; }
    public string? GitHubUsername { get; init; }
    public string? TwitterHandle { get; init; }
    public string? Website { get; init; }
    public string? Bio { get; init; }
    public string? AvatarUrl { get; init; }
}