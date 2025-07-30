using MCPHub.Domain.Messaging;

namespace MCPHub.Domain.Events;

/// <summary>
/// Event raised when a user verifies their email address
/// </summary>
public record UserEmailVerifiedEvent : BaseEvent {
    public UserEmailVerifiedEvent(string userId, string userName, string email)
        : base("User") {
        UserId = userId;
        UserName = userName;
        Email = email;
        AggregateId = userId;
    }

    public UserEmailVerifiedEvent(string userId, string userName, string email, string? correlationId, string? initiatedBy)
        : base("User", correlationId, initiatedBy, userId) {
        UserId = userId;
        UserName = userName;
        Email = email;
    }

    /// <summary>
    /// The ID of the user who verified their email
    /// </summary>
    public new string UserId { get; init; }

    /// <summary>
    /// The username of the user who verified their email
    /// </summary>
    public string UserName { get; init; }

    /// <summary>
    /// The verified email address
    /// </summary>
    public string Email { get; init; }
}