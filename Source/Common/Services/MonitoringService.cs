namespace MCPHub.Common.Services;

/// <summary>
/// Monitoring service skeleton implementation
/// Following contracts-first approach - implementation when needed
/// </summary>
public class MonitoringService : IMonitoringService {
    public void RecordHttpRequest(string method, string path, int statusCode, TimeSpan duration)
        => throw new NotImplementedException("HTTP request monitoring will be implemented when web request tracking is needed");

    public void RecordDatabaseQuery(string operation, string table, TimeSpan duration, bool success)
        => throw new NotImplementedException("Database query monitoring will be implemented when database performance tracking is needed");

    public void RecordCacheOperation(string operation, TimeSpan duration, bool hit)
        => throw new NotImplementedException("Cache operation monitoring will be implemented when cache performance tracking is needed");

    public void RecordSearchOperation(string operation, TimeSpan duration, int resultCount)
        => throw new NotImplementedException("Search operation monitoring will be implemented when search performance tracking is needed");

    public void RecordMessageProcessing(string messageType, TimeSpan duration, bool success)
        => throw new NotImplementedException("Message processing monitoring will be implemented when messaging performance tracking is needed");

    public void RecordSecurityScan(string scanType, TimeSpan duration, string result)
        => throw new NotImplementedException("Security scan monitoring will be implemented when security performance tracking is needed");

    public void RecordError(string component, string errorType, string message)
        => throw new NotImplementedException("Error recording will be implemented when error tracking is needed");

    public IDisposable StartActivity(string operationName)
        => throw new NotImplementedException("Activity tracking will be implemented when operation monitoring is needed");
}