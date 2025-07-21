namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// Result of email verification operation
/// </summary>
public class EmailVerificationResult {
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
}