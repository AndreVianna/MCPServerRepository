using Common.Extensions;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Common.Services;

/// <summary>
/// Monitoring service for application metrics and performance tracking
/// </summary>
public interface IMonitoringService
{
    void RecordHttpRequest(string method, string path, int statusCode, TimeSpan duration);
    void RecordDatabaseQuery(string operation, string table, TimeSpan duration, bool success);
    void RecordCacheOperation(string operation, TimeSpan duration, bool hit);
    void RecordSearchOperation(string operation, TimeSpan duration, int resultCount);
    void RecordMessageProcessing(string messageType, TimeSpan duration, bool success);
    void RecordSecurityScan(string scanType, TimeSpan duration, string result);
    void RecordError(string component, string errorType, string message);
    IDisposable StartActivity(string operationName);
}

public class MonitoringService : IMonitoringService
{
    private readonly Counter<long> _httpRequestCounter;
    private readonly Histogram<double> _httpRequestDuration;
    private readonly Counter<long> _databaseQueryCounter;
    private readonly Histogram<double> _databaseQueryDuration;
    private readonly Counter<long> _cacheOperationCounter;
    private readonly Histogram<double> _cacheOperationDuration;
    private readonly Counter<long> _searchOperationCounter;
    private readonly Histogram<double> _searchOperationDuration;
    private readonly Counter<long> _messageProcessingCounter;
    private readonly Histogram<double> _messageProcessingDuration;
    private readonly Counter<long> _securityScanCounter;
    private readonly Histogram<double> _securityScanDuration;
    private readonly Counter<long> _errorCounter;
    private readonly Gauge<long> _activeConnections;
    private readonly Gauge<long> _memoryUsage;
    private readonly ILogger<MonitoringService> _logger;

    public MonitoringService(ILogger<MonitoringService> logger)
    {
        _logger = logger;

        // HTTP metrics
        _httpRequestCounter = Meters.Main.CreateCounter<long>(
            "http_requests_total",
            "Total number of HTTP requests");
        
        _httpRequestDuration = Meters.Main.CreateHistogram<double>(
            "http_request_duration_seconds",
            "HTTP request duration in seconds");

        // Database metrics
        _databaseQueryCounter = Meters.Database.CreateCounter<long>(
            "database_queries_total",
            "Total number of database queries");
        
        _databaseQueryDuration = Meters.Database.CreateHistogram<double>(
            "database_query_duration_seconds",
            "Database query duration in seconds");

        // Cache metrics
        _cacheOperationCounter = Meters.Cache.CreateCounter<long>(
            "cache_operations_total",
            "Total number of cache operations");
        
        _cacheOperationDuration = Meters.Cache.CreateHistogram<double>(
            "cache_operation_duration_seconds",
            "Cache operation duration in seconds");

        // Search metrics
        _searchOperationCounter = Meters.Search.CreateCounter<long>(
            "search_operations_total",
            "Total number of search operations");
        
        _searchOperationDuration = Meters.Search.CreateHistogram<double>(
            "search_operation_duration_seconds",
            "Search operation duration in seconds");

        // Message processing metrics
        _messageProcessingCounter = Meters.MessageQueue.CreateCounter<long>(
            "message_processing_total",
            "Total number of processed messages");
        
        _messageProcessingDuration = Meters.MessageQueue.CreateHistogram<double>(
            "message_processing_duration_seconds",
            "Message processing duration in seconds");

        // Security metrics
        _securityScanCounter = Meters.Security.CreateCounter<long>(
            "security_scans_total",
            "Total number of security scans");
        
        _securityScanDuration = Meters.Security.CreateHistogram<double>(
            "security_scan_duration_seconds",
            "Security scan duration in seconds");

        // Error metrics
        _errorCounter = Meters.Main.CreateCounter<long>(
            "errors_total",
            "Total number of errors");

        // System metrics
        _activeConnections = Meters.Main.CreateGauge<long>(
            "active_connections",
            "Number of active connections");
        
        _memoryUsage = Meters.Main.CreateGauge<long>(
            "memory_usage_bytes",
            "Memory usage in bytes");

        // Start background metrics collection
        _ = Task.Run(CollectSystemMetrics);
    }

    public void RecordHttpRequest(string method, string path, int statusCode, TimeSpan duration)
    {
        _httpRequestCounter.Add(1, 
            new KeyValuePair<string, object?>("method", method),
            new KeyValuePair<string, object?>("path", path),
            new KeyValuePair<string, object?>("status_code", statusCode));

        _httpRequestDuration.Record(duration.TotalSeconds,
            new KeyValuePair<string, object?>("method", method),
            new KeyValuePair<string, object?>("path", path),
            new KeyValuePair<string, object?>("status_code", statusCode));
    }

    public void RecordDatabaseQuery(string operation, string table, TimeSpan duration, bool success)
    {
        _databaseQueryCounter.Add(1,
            new KeyValuePair<string, object?>("operation", operation),
            new KeyValuePair<string, object?>("table", table),
            new KeyValuePair<string, object?>("success", success));

        _databaseQueryDuration.Record(duration.TotalSeconds,
            new KeyValuePair<string, object?>("operation", operation),
            new KeyValuePair<string, object?>("table", table),
            new KeyValuePair<string, object?>("success", success));
    }

    public void RecordCacheOperation(string operation, TimeSpan duration, bool hit)
    {
        _cacheOperationCounter.Add(1,
            new KeyValuePair<string, object?>("operation", operation),
            new KeyValuePair<string, object?>("hit", hit));

        _cacheOperationDuration.Record(duration.TotalSeconds,
            new KeyValuePair<string, object?>("operation", operation),
            new KeyValuePair<string, object?>("hit", hit));
    }

    public void RecordSearchOperation(string operation, TimeSpan duration, int resultCount)
    {
        _searchOperationCounter.Add(1,
            new KeyValuePair<string, object?>("operation", operation),
            new KeyValuePair<string, object?>("result_count", resultCount));

        _searchOperationDuration.Record(duration.TotalSeconds,
            new KeyValuePair<string, object?>("operation", operation),
            new KeyValuePair<string, object?>("result_count", resultCount));
    }

    public void RecordMessageProcessing(string messageType, TimeSpan duration, bool success)
    {
        _messageProcessingCounter.Add(1,
            new KeyValuePair<string, object?>("message_type", messageType),
            new KeyValuePair<string, object?>("success", success));

        _messageProcessingDuration.Record(duration.TotalSeconds,
            new KeyValuePair<string, object?>("message_type", messageType),
            new KeyValuePair<string, object?>("success", success));
    }

    public void RecordSecurityScan(string scanType, TimeSpan duration, string result)
    {
        _securityScanCounter.Add(1,
            new KeyValuePair<string, object?>("scan_type", scanType),
            new KeyValuePair<string, object?>("result", result));

        _securityScanDuration.Record(duration.TotalSeconds,
            new KeyValuePair<string, object?>("scan_type", scanType),
            new KeyValuePair<string, object?>("result", result));
    }

    public void RecordError(string component, string errorType, string message)
    {
        _errorCounter.Add(1,
            new KeyValuePair<string, object?>("component", component),
            new KeyValuePair<string, object?>("error_type", errorType));

        _logger.LogError("Error recorded: Component={Component}, Type={ErrorType}, Message={Message}", 
            component, errorType, message);
    }

    public IDisposable StartActivity(string operationName)
    {
        var activity = ActivitySources.Main.StartActivity(operationName);
        return new ActivityScope(activity);
    }

    private async Task CollectSystemMetrics()
    {
        while (true)
        {
            try
            {
                var memoryUsage = GC.GetTotalMemory(false);
                _memoryUsage.Record(memoryUsage);

                await Task.Delay(TimeSpan.FromSeconds(30));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error collecting system metrics");
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }
    }

    private class ActivityScope : IDisposable
    {
        private readonly Activity? _activity;

        public ActivityScope(Activity? activity)
        {
            _activity = activity;
        }

        public void Dispose()
        {
            _activity?.Dispose();
        }
    }
}