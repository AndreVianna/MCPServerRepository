using MCPHub.Domain.Messaging;

namespace MCPHub.Domain.Events;

/// <summary>
/// Event raised when a new user registers
/// </summary>
public record UserRegisteredEvent : BaseEvent {
    public UserRegisteredEvent(string userId, string userName, string email)
        : base("User") {
        UserId = userId;
        UserName = userName;
        Email = email;
        AggregateId = userId;
    }

    public UserRegisteredEvent(string userId, string userName, string email, string? correlationId, string? initiatedBy)
        : base("User", correlationId, initiatedBy, userId) {
        UserId = userId;
        UserName = userName;
        Email = email;
    }

    /// <summary>
    /// The ID of the registered user
    /// </summary>
    public new string UserId { get; init; }

    /// <summary>
    /// The username of the registered user
    /// </summary>
    public string UserName { get; init; }

    /// <summary>
    /// The email of the registered user
    /// </summary>
    public string Email { get; init; }
}