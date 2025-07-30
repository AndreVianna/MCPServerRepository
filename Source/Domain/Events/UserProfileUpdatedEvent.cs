using MCPHub.Domain.Messaging;

namespace MCPHub.Domain.Events;

/// <summary>
/// Event raised when a user updates their profile
/// </summary>
public record UserProfileUpdatedEvent : BaseEvent {
    public UserProfileUpdatedEvent(string userId, string userName, Dictionary<string, object?> updatedFields)
        : base("User") {
        UserId = userId;
        UserName = userName;
        UpdatedFields = updatedFields;
        AggregateId = userId;
    }

    public UserProfileUpdatedEvent(string userId, string userName, Dictionary<string, object?> updatedFields, string? correlationId, string? initiatedBy)
        : base("User", correlationId, initiatedBy, userId) {
        UserId = userId;
        UserName = userName;
        UpdatedFields = updatedFields;
    }

    /// <summary>
    /// The ID of the user who updated their profile
    /// </summary>
    public new string UserId { get; init; }

    /// <summary>
    /// The username of the user who updated their profile
    /// </summary>
    public string UserName { get; init; }

    /// <summary>
    /// Dictionary of fields that were updated with their new values
    /// </summary>
    public Dictionary<string, object?> UpdatedFields { get; init; }
}