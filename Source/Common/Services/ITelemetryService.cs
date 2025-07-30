namespace MCPHub.Common.Services;

/// <summary>
/// Telemetry and observability service
/// Supports: Console → Application Insights → OpenTelemetry
/// </summary>
public interface ITelemetryService {
    /// <summary>
    /// Records a custom metric
    /// </summary>
    void RecordMetric(string name, double value, IDictionary<string, string>? tags = null);

    /// <summary>
    /// Tracks an event
    /// </summary>
    void TrackEvent(string name, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null);

    /// <summary>
    /// Tracks a dependency call
    /// </summary>
    void TrackDependency(string dependencyType, string target, string command, DateTimeOffset startTime, TimeSpan duration, bool success);

    /// <summary>
    /// Tracks an exception
    /// </summary>
    void TrackException(Exception exception, IDictionary<string, string>? properties = null);

    /// <summary>
    /// Starts a traced operation
    /// </summary>
    ITracedOperation StartOperation(string operationName, IDictionary<string, string>? properties = null);

    /// <summary>
    /// Creates a timer for duration tracking
    /// </summary>
    IDisposable TrackDuration(string metricName, IDictionary<string, string>? tags = null);

    /// <summary>
    /// Gets collected metrics for analysis
    /// </summary>
    Task<IEnumerable<MetricSnapshot>> GetMetricsAsync(string metricName, TimeSpan period, CancellationToken cancellationToken = default);

    /// <summary>
    /// Configures alerts on metrics
    /// </summary>
    Task ConfigureAlertAsync(string metricName, MetricAlert alert, CancellationToken cancellationToken = default);
}

/// <summary>
/// Traced operation interface
/// </summary>
public interface ITracedOperation : IDisposable {
    string OperationId { get; }
    void SetProperty(string key, string value);
    void SetMetric(string key, double value);
    void SetSuccess(bool success);
}

/// <summary>
/// Metric snapshot for analysis
/// </summary>
public record MetricSnapshot(
    string Name,
    double Value,
    IDictionary<string, string> Tags,
    DateTime Timestamp);

/// <summary>
/// Metric alert configuration
/// </summary>
public record MetricAlert(
    double Threshold,
    MetricAlertCondition Condition,
    string NotificationEndpoint,
    TimeSpan EvaluationWindow);

/// <summary>
/// Metric alert conditions
/// </summary>
public enum MetricAlertCondition {
    GreaterThan,
    LessThan,
    EqualTo,
    NotEqualTo,
}