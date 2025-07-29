namespace MCPHub.Domain.Common;

public record AuditEntry : IAuditEntry, IValidatableObject {
    public required string Action { get; init; } = string.Empty;
    public required Guid UserId { get; init; }
    public required DateTimeOffset DateTime { get; init; } = DateTimeOffset.UtcNow;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
        if (string.IsNullOrWhiteSpace(Action))
            yield return new ValidationResult("Action cannot be null or empty.", [nameof(Action)]);
    }
}