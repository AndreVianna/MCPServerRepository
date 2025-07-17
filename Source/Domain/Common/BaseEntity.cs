namespace Domain.Common;

/// <summary>
/// Base class for all domain entities
/// </summary>
public abstract class BaseEntity {
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;
    public string? CreatedBy { get; protected set; }
    public string? UpdatedBy { get; protected set; }

    protected void SetCreatedBy(string userId) => CreatedBy = userId;

    protected void SetUpdatedBy(string userId) {
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;
    }
}