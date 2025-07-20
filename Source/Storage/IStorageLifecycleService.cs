namespace MCPHub.Storage;

/// <summary>
/// Service for managing storage lifecycle policies and retention
/// </summary>
public interface IStorageLifecycleService {
    /// <summary>
    /// Applies lifecycle policies to all containers
    /// </summary>
    Task ApplyLifecyclePoliciesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies lifecycle policies to a specific container
    /// </summary>
    Task ApplyLifecyclePoliciesAsync(string containerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a lifecycle policy
    /// </summary>
    Task<bool> ValidateLifecyclePolicyAsync(StorageLifecyclePolicy policy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets lifecycle policy statistics
    /// </summary>
    Task<StorageLifecycleStatistics> GetLifecycleStatisticsAsync(CancellationToken cancellationToken = default);
}
