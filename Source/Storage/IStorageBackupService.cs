namespace MCPHub.Storage;

/// <summary>
/// Service for managing storage backup and disaster recovery
/// </summary>
public interface IStorageBackupService {
    /// <summary>
    /// Creates a backup of a container
    /// </summary>
    Task<StorageBackupResult> CreateBackupAsync(string containerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores a container from backup
    /// </summary>
    Task<StorageRestoreResult> RestoreBackupAsync(string backupId, string? targetContainerName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists available backups
    /// </summary>
    Task<List<StorageBackupInfo>> ListBackupsAsync(string? containerName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a backup
    /// </summary>
    Task DeleteBackupAsync(string backupId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates backup integrity
    /// </summary>
    Task<StorageBackupValidationResult> ValidateBackupAsync(string backupId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs disaster recovery operation
    /// </summary>
    Task<StorageDisasterRecoveryResult> PerformDisasterRecoveryAsync(StorageDisasterRecoveryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets backup statistics
    /// </summary>
    Task<StorageBackupStatistics> GetBackupStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Tests disaster recovery readiness
    /// </summary>
    Task<StorageDisasterRecoveryTestResult> TestDisasterRecoveryAsync(CancellationToken cancellationToken = default);
}
