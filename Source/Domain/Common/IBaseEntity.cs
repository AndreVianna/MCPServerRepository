namespace MCPHub.Domain.Common;

public interface IBaseEntity {
    Guid Id { get; }
    ICollection<AuditEntry> AuditTrail { get; }
}