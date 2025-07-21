namespace MCPHub.Domain.Common;

public interface IAuditEntry {
    string Action { get; }
    Guid UserId { get; }
    DateTimeOffset DateTime { get; }
}