using System.Diagnostics;

namespace MCPHub.Common.Services;

/// <summary>
/// Distributed tracing service for OpenTelemetry integration
/// Supports: Development (Console) → Production (OTLP) → Enterprise (Multiple exporters)
/// </summary>
public interface IDistributedTracingService
{
    /// <summary>
    /// Starts a new traced activity
    /// </summary>
    Activity? StartActivity(string operationName, ActivityKind kind = ActivityKind.Internal);
    
    /// <summary>
    /// Creates a child activity from the current context
    /// </summary>
    Activity? StartChildActivity(string operationName, ActivityContext? parentContext = null);
    
    /// <summary>
    /// Adds tags to the current activity
    /// </summary>
    void AddTag(string key, string value);
    
    /// <summary>
    /// Adds tags to a specific activity
    /// </summary>
    void AddTag(Activity activity, string key, string value);
    
    /// <summary>
    /// Records an event in the current activity
    /// </summary>
    void AddEvent(string name, IDictionary<string, object?>? attributes = null);
    
    /// <summary>
    /// Records an event in a specific activity
    /// </summary>
    void AddEvent(Activity activity, string name, IDictionary<string, object?>? attributes = null);
    
    /// <summary>
    /// Sets the status of the current activity
    /// </summary>
    void SetStatus(ActivityStatusCode statusCode, string? description = null);
    
    /// <summary>
    /// Sets the status of a specific activity
    /// </summary>
    void SetStatus(Activity activity, ActivityStatusCode statusCode, string? description = null);
    
    /// <summary>
    /// Records an exception in the current activity
    /// </summary>
    void RecordException(Exception exception, IDictionary<string, object?>? attributes = null);
    
    /// <summary>
    /// Records an exception in a specific activity
    /// </summary>
    void RecordException(Activity activity, Exception exception, IDictionary<string, object?>? attributes = null);
    
    /// <summary>
    /// Gets the current trace context for propagation
    /// </summary>
    ActivityContext GetCurrentContext();
    
    /// <summary>
    /// Creates a trace context from external headers (for HTTP requests)
    /// </summary>
    ActivityContext? ExtractContext(IDictionary<string, string> headers);
    
    /// <summary>
    /// Injects trace context into headers (for HTTP requests)
    /// </summary>
    void InjectContext(ActivityContext context, IDictionary<string, string> headers);
    
    /// <summary>
    /// Creates a correlation ID for request tracking
    /// </summary>
    string GenerateCorrelationId();
    
    /// <summary>
    /// Gets the current correlation ID
    /// </summary>
    string? GetCurrentCorrelationId();
    
    /// <summary>
    /// Sets correlation ID for the current context
    /// </summary>
    void SetCorrelationId(string correlationId);
}

/// <summary>
/// MCP-specific tracing extensions for business operations
/// </summary>
public interface IMCPTracingService : IDistributedTracingService
{
    /// <summary>
    /// Traces package download operation
    /// </summary>
    Activity? TracePackageDownload(string packageName, string version, string userId);
    
    /// <summary>
    /// Traces package installation operation
    /// </summary>
    Activity? TracePackageInstallation(string packageName, string version, string userId);
    
    /// <summary>
    /// Traces security scan operation
    /// </summary>
    Activity? TraceSecurityScan(string packageName, string version, string scanType);
    
    /// <summary>
    /// Traces search operation
    /// </summary>
    Activity? TraceSearchOperation(string query, string userId, int resultCount);
    
    /// <summary>
    /// Traces authentication operation
    /// </summary>
    Activity? TraceAuthentication(string operation, string userId, bool success);
    
    /// <summary>
    /// Traces database operation
    /// </summary>
    Activity? TraceDatabaseOperation(string operation, string table, TimeSpan duration);
    
    /// <summary>
    /// Traces cache operation
    /// </summary>
    Activity? TraceCacheOperation(string operation, string key, bool hit);
    
    /// <summary>
    /// Traces external API call
    /// </summary>
    Activity? TraceExternalApiCall(string service, string endpoint, string method);
    
    /// <summary>
    /// Traces message processing operation
    /// </summary>
    Activity? TraceMessageProcessing(string messageType, string source, string destination);
}

/// <summary>
/// Trace sampling configuration for performance optimization
/// </summary>
public record TraceSamplingConfig(
    double DefaultSamplingRatio,
    Dictionary<string, double> OperationSamplingRules,
    List<string> AlwaysSampleOperations,
    List<string> NeverSampleOperations
);

/// <summary>
/// Trace export configuration for different environments
/// </summary>
public record TraceExportConfig(
    TraceExportMode ExportMode,
    string? OtlpEndpoint,
    string? JaegerEndpoint,
    string? ZipkinEndpoint,
    TimeSpan ExportTimeout,
    int MaxExportBatchSize
);

/// <summary>
/// Trace export modes for different deployment scenarios
/// </summary>
public enum TraceExportMode
{
    Console,        // Development - console output
    Otlp,          // Production - OpenTelemetry Protocol
    Jaeger,        // Alternative - Jaeger collector
    Zipkin,        // Alternative - Zipkin collector
    Multiple       // Enterprise - multiple exporters
}