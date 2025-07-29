namespace MCPHub.Storage;

/// <summary>
/// Storage service for managing file operations in cloud storage
/// </summary>
public interface IStorageService {
    /// <summary>
    /// Uploads a file to storage
    /// </summary>
    /// <param name="containerName">Container or bucket name</param>
    /// <param name="fileName">File name/path</param>
    /// <param name="content">File content stream</param>
    /// <param name="contentType">MIME type of the content</param>
    /// <param name="metadata">Optional metadata dictionary</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Storage URI of the uploaded file</returns>
    Task<string> UploadAsync(
        string containerName,
        string fileName,
        Stream content,
        string contentType,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from storage
    /// </summary>
    /// <param name="containerName">Container or bucket name</param>
    /// <param name="fileName">File name/path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File content stream</returns>
    Task<Stream> DownloadAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file to a specific path
    /// </summary>
    /// <param name="containerName">Container or bucket name</param>
    /// <param name="fileName">File name/path</param>
    /// <param name="destinationPath">Local destination path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DownloadToFileAsync(
        string containerName,
        string fileName,
        string destinationPath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from storage
    /// </summary>
    /// <param name="containerName">Container or bucket name</param>
    /// <param name="fileName">File name/path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes multiple files from storage
    /// </summary>
    /// <param name="containerName">Container or bucket name</param>
    /// <param name="fileNames">List of file names/paths to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteBatchAsync(
        string containerName,
        IEnumerable<string> fileNames,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a file exists in storage
    /// </summary>
    /// <param name="containerName">Container or bucket name</param>
    /// <param name="fileName">File name/path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if file exists, false otherwise</returns>
    Task<bool> ExistsAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets file metadata
    /// </summary>
    /// <param name="containerName">Container or bucket name</param>
    /// <param name="fileName">File name/path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File metadata</returns>
    Task<StorageFileMetadata> GetMetadataAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists files in a container with optional prefix filter
    /// </summary>
    /// <param name="containerName">Container or bucket name</param>
    /// <param name="prefix">Optional prefix filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of file information</returns>
    Task<IEnumerable<StorageFileInfo>> ListFilesAsync(
        string containerName,
        string? prefix = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a pre-signed URL for file access
    /// </summary>
    /// <param name="containerName">Container or bucket name</param>
    /// <param name="fileName">File name/path</param>
    /// <param name="expiration">URL expiration time</param>
    /// <param name="permissions">Access permissions</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Pre-signed URL</returns>
    Task<string> GetPresignedUrlAsync(
        string containerName,
        string fileName,
        TimeSpan expiration,
        StoragePermissions permissions = StoragePermissions.Read,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a container if it doesn't exist
    /// </summary>
    /// <param name="containerName">Container or bucket name</param>
    /// <param name="isPublic">Whether the container should be public</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task CreateContainerAsync(
        string containerName,
        bool isPublic = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a container and all its contents
    /// </summary>
    /// <param name="containerName">Container or bucket name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteContainerAsync(
        string containerName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Copies a file within storage
    /// </summary>
    /// <param name="sourceContainer">Source container name</param>
    /// <param name="sourceFileName">Source file name/path</param>
    /// <param name="destinationContainer">Destination container name</param>
    /// <param name="destinationFileName">Destination file name/path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task CopyAsync(
        string sourceContainer,
        string sourceFileName,
        string destinationContainer,
        string destinationFileName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets storage usage statistics
    /// </summary>
    /// <param name="containerName">Container or bucket name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Storage usage statistics</returns>
    Task<StorageUsageInfo> GetUsageAsync(
        string containerName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the storage provider type
    /// </summary>
    StorageProviderType ProviderType { get; }

    /// <summary>
    /// Checks storage service health
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Storage health information</returns>
    Task<StorageHealthInfo> CheckHealthAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets storage performance metrics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Storage performance metrics</returns>
    Task<StoragePerformanceMetrics> GetPerformanceMetricsAsync(CancellationToken cancellationToken = default);
}