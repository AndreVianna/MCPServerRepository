namespace MCPHub.Common.Services;

/// <summary>
/// Feature flag service for progressive rollouts
/// Supports: Configuration → Database → Azure App Configuration
/// </summary>
public interface IFeatureFlagService {
    /// <summary>
    /// Checks if a feature is enabled for a user/context
    /// </summary>
    Task<bool> IsEnabledAsync(string featureName, FeatureContext? context = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets feature configuration value
    /// </summary>
    Task<T?> GetConfigurationAsync<T>(string featureName, T defaultValue, FeatureContext? context = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all feature flags for a context
    /// </summary>
    Task<IDictionary<string, bool>> GetAllFlagsAsync(FeatureContext? context = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a feature flag configuration
    /// </summary>
    Task SetFeatureFlagAsync(string featureName, FeatureFlagConfiguration configuration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tracks feature usage for analytics
    /// </summary>
    Task TrackUsageAsync(string featureName, FeatureContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets feature usage statistics
    /// </summary>
    Task<FeatureUsageStatistics> GetUsageStatisticsAsync(string featureName, TimeSpan period, CancellationToken cancellationToken = default);
}

/// <summary>
/// Feature evaluation context
/// </summary>
public record FeatureContext(
    string? UserId = null,
    string? SessionId = null,
    IDictionary<string, string>? Properties = null,
    DateTimeOffset? Timestamp = null);

/// <summary>
/// Feature flag configuration
/// </summary>
public record FeatureFlagConfiguration(
    bool IsEnabled,
    double RolloutPercentage,
    IEnumerable<FeatureRule>? Rules = null,
    DateTimeOffset? StartDate = null,
    DateTimeOffset? EndDate = null);

/// <summary>
/// Feature evaluation rule
/// </summary>
public record FeatureRule(
    string Property,
    FeatureRuleOperator Operator,
    string Value);

/// <summary>
/// Feature rule operators
/// </summary>
public enum FeatureRuleOperator {
    Equals,
    NotEquals,
    Contains,
    StartsWith,
    In,
    GreaterThan,
    LessThan
}

/// <summary>
/// Feature usage statistics
/// </summary>
public record FeatureUsageStatistics(
    string FeatureName,
    long TotalRequests,
    long EnabledRequests,
    double EnabledPercentage,
    IDictionary<string, long> ContextBreakdown,
    DateTime PeriodStart,
    DateTime PeriodEnd);