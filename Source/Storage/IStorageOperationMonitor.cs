namespace MCPHub.Storage;

/// <summary>
/// Interface for monitoring individual storage operations
/// </summary>
public interface IStorageOperationMonitor : IDisposable {
    /// <summary>
    /// Records operation success
    /// </summary>
    void RecordSuccess(long? bytesTransferred = null);

    /// <summary>
    /// Records operation failure
    /// </summary>
    void RecordFailure(Exception exception);

    /// <summary>
    /// Records operation completion
    /// </summary>
    void RecordCompletion(bool success, long? bytesTransferred = null, Exception? exception = null);
}