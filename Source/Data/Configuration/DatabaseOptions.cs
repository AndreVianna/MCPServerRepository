using System.ComponentModel.DataAnnotations;

namespace MCPHub.Data.Configuration;

public class DatabaseOptions : IValidatableObject {
    public const string SectionName = "Database";

    [Required(ErrorMessage = "Database connection string is required")]
    public string ConnectionString { get; set; } = string.Empty;
    [Range(0, int.MaxValue, ErrorMessage = "Database MaxRetryCount must be non-negative")]
    public int MaxRetryCount { get; set; } = 3;
    public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public bool EnableSensitiveDataLogging { get; set; } = false;
    public bool EnableDetailedErrors { get; set; } = false;
    [Range(1, int.MaxValue, ErrorMessage = "Database MaxPoolSize must be positive")]
    public int MaxPoolSize { get; set; } = 100;
    public TimeSpan HealthCheckTimeout { get; set; } = TimeSpan.FromSeconds(5);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
        if (CommandTimeout <= TimeSpan.Zero) {
            yield return new ValidationResult("Database CommandTimeout must be positive", [nameof(CommandTimeout)]);
        }

        if (HealthCheckTimeout <= TimeSpan.Zero) {
            yield return new ValidationResult("Database HealthCheckTimeout must be positive", [nameof(HealthCheckTimeout)]);
        }
    }
}