using System.ComponentModel.DataAnnotations;

namespace MCPHub.Common.Configuration;

public class CacheOptions : IValidatableObject {
    public const string SectionName = "Cache";

    [Required(ErrorMessage = "Cache connection string is required")]
    public string ConnectionString { get; set; } = string.Empty;
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);
    public TimeSpan SlidingExpiration { get; set; } = TimeSpan.FromMinutes(5);
    [Range(0, int.MaxValue, ErrorMessage = "Cache Database must be non-negative")]
    public int Database { get; set; } = 0;
    [Required(ErrorMessage = "Cache KeyPrefix is required")]
    public string KeyPrefix { get; set; } = "mcphub:";
    public bool EnableCompression { get; set; } = true;
    public TimeSpan HealthCheckTimeout { get; set; } = TimeSpan.FromSeconds(5);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
        if (DefaultExpiration <= TimeSpan.Zero) {
            yield return new ValidationResult("Cache DefaultExpiration must be positive", [nameof(DefaultExpiration)]);
        }

        if (SlidingExpiration <= TimeSpan.Zero) {
            yield return new ValidationResult("Cache SlidingExpiration must be positive", [nameof(SlidingExpiration)]);
        }

        if (HealthCheckTimeout <= TimeSpan.Zero) {
            yield return new ValidationResult("Cache HealthCheckTimeout must be positive", [nameof(HealthCheckTimeout)]);
        }
    }
}