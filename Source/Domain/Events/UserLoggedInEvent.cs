using MCPHub.Domain.Messaging;

namespace MCPHub.Domain.Events;

/// <summary>
/// Event raised when a user successfully logs in
/// </summary>
public record UserLoggedInEvent : BaseEvent {
    public UserLoggedInEvent(string userId, string userName, string? ipAddress = null, string? userAgent = null)
        : base("User", 1) {
        UserId = userId;
        UserName = userName;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        AggregateId = userId;
    }

    public UserLoggedInEvent(string userId, string userName, string? correlationId, string? initiatedBy, string? ipAddress = null, string? userAgent = null)
        : base("User", correlationId, initiatedBy, userId, 1) {
        UserId = userId;
        UserName = userName;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }

    /// <summary>
    /// The ID of the logged in user
    /// </summary>
    public new string UserId { get; init; }

    /// <summary>
    /// The username of the logged in user
    /// </summary>
    public string UserName { get; init; }

    /// <summary>
    /// The IP address of the login attempt
    /// </summary>
    public string? IpAddress { get; init; }

    /// <summary>
    /// The user agent of the login attempt
    /// </summary>
    public string? UserAgent { get; init; }
}