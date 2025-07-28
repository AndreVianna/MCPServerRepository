namespace MCPHub.Domain.Common;

public abstract class BaseEntity : IBaseEntity {
    public Guid Id { get; protected set; } = Guid.CreateVersion7();
    public ICollection<IAuditEntry> AuditTrail { get; } = [];
    ICollection<AuditEntry> IBaseEntity.AuditTrail => AuditTrail.Cast<AuditEntry>().ToList();
}