namespace MCPHub.Common.Services;

/// <summary>
/// Monitoring service for application metrics and performance tracking
/// </summary>
public interface IMonitoringService {
    void RecordHttpRequest(string method, string path, int statusCode, TimeSpan duration);
    void RecordDatabaseQuery(string operation, string table, TimeSpan duration, bool success);
    void RecordCacheOperation(string operation, TimeSpan duration, bool hit);
    void RecordSearchOperation(string operation, TimeSpan duration, int resultCount);
    void RecordMessageProcessing(string messageType, TimeSpan duration, bool success);
    void RecordSecurityScan(string scanType, TimeSpan duration, string result);
    void RecordError(string component, string errorType, string message);
    IDisposable StartActivity(string operationName);
}