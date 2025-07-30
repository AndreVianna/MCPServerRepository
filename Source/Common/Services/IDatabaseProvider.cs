namespace MCPHub.Common.Services;

/// <summary>
/// Database provider abstraction for technology deferral
/// Supports: SQLite → PostgreSQL → PostgreSQL Cluster
/// </summary>
public interface IDatabaseProvider {
    /// <summary>
    /// Gets the connection string for the current provider
    /// </summary>
    string ConnectionString { get; }

    /// <summary>
    /// Gets the provider type (SQLite, PostgreSQL, etc.)
    /// </summary>
    DatabaseProviderType ProviderType { get; }

    /// <summary>
    /// Executes raw SQL with parameters
    /// </summary>
    Task<T> ExecuteScalarAsync<T>(string sql, object? parameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes bulk operations efficiently
    /// </summary>
    Task<int> ExecuteBulkAsync<T>(string sql, IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks database health and connectivity
    /// </summary>
    Task<DatabaseHealthInfo> CheckHealthAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets performance metrics for migration decisions
    /// </summary>
    Task<DatabasePerformanceMetrics> GetPerformanceMetricsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates migration readiness to next tier
    /// </summary>
    Task<MigrationReadinessResult> ValidateMigrationReadinessAsync(DatabaseProviderType targetProvider, CancellationToken cancellationToken = default);
}

/// <summary>
/// Database provider types for technology progression
/// </summary>
public enum DatabaseProviderType {
    SQLite,           // Development tier
    PostgreSQL,       // Production tier
    PostgreSQLCluster, // Enterprise tier
}

/// <summary>
/// Database health information
/// </summary>
public record DatabaseHealthInfo(
    bool IsHealthy,
    TimeSpan ResponseTime,
    long DatabaseSize,
    int ConnectionCount,
    string? ErrorMessage = null);

/// <summary>
/// Database performance metrics
/// </summary>
public record DatabasePerformanceMetrics(
    double AverageQueryTime,
    long QueriesPerSecond,
    double CpuUsage,
    double MemoryUsage,
    long DiskSpace,
    DateTime Timestamp);

/// <summary>
/// Migration readiness assessment
/// </summary>
public record MigrationReadinessResult(
    bool IsReady,
    IEnumerable<string> RequiredActions,
    EstimatedMigrationTime EstimatedTime,
    IEnumerable<string> Warnings);

/// <summary>
/// Estimated migration time
/// </summary>
public record EstimatedMigrationTime(
    TimeSpan MinimumTime,
    TimeSpan MaximumTime);