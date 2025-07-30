namespace MCPHub.Common.Providers;

/// <summary>
/// Configuration for infrastructure providers
/// </summary>
public record ProviderConfiguration(
    string ProviderType,
    IDictionary<string, string> Settings,
    EnvironmentTier Tier);

/// <summary>
/// Information about available provider types
/// </summary>
public record ProviderTypeInfo(
    string TypeName,
    string Description,
    EnvironmentTier MinimumTier,
    IEnumerable<string> RequiredSettings,
    EstimatedCost CostEstimate);

/// <summary>
/// Result of provider configuration validation
/// </summary>
public record ProviderValidationResult(
    bool IsValid,
    IEnumerable<string> Errors,
    IEnumerable<string> Warnings);

/// <summary>
/// Result of provider connectivity testing
/// </summary>
public record ProviderTestResult(
    bool IsSuccessful,
    TimeSpan ResponseTime,
    string? ErrorMessage,
    IDictionary<string, object> Diagnostics);

/// <summary>
/// Environment tier for cost optimization and technology progression
/// </summary>
public enum EnvironmentTier {
    Development,  // Free tier
    Production,   // < $100/month
    Enterprise, // $500+/month
}

/// <summary>
/// Cost estimation for provider usage
/// </summary>
public record EstimatedCost(
    decimal MonthlyBaseCost,
    decimal CostPerOperation,
    string Currency = "USD");

/// <summary>
/// Usage projections for cost analysis
/// </summary>
public record UsageProjection(
    long MonthlyRequests,
    long StorageGigabytes,
    long DatabaseOperations,
    long CacheOperations,
    long BackgroundJobs);

/// <summary>
/// Cost projection over time
/// </summary>
public record CostProjection(
    decimal Month1,
    decimal Month2,
    decimal Month3,
    decimal AverageMonthly);

/// <summary>
/// Cost optimization recommendation
/// </summary>
public record CostOptimizationRecommendation(
    string Description,
    decimal PotentialSavings,
    ImplementationComplexity Complexity,
    string ActionRequired);

/// <summary>
/// Implementation complexity levels
/// </summary>
public enum ImplementationComplexity {
    Low,
    Medium,
    High,
}