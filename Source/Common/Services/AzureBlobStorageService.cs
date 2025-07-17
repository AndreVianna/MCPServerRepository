using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

using Common.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Services;

/// <summary>
/// Azure Blob Storage implementation of IStorageService
/// </summary>
public class AzureBlobStorageService : IStorageService {
    private readonly BlobServiceClient _blobServiceClient;
    private readonly StorageConfiguration _configuration;
    private readonly ILogger<AzureBlobStorageService> _logger;

    public AzureBlobStorageService(
        IOptions<StorageConfiguration> configuration,
        ILogger<AzureBlobStorageService> logger) {
        _configuration = configuration.Value;
        _logger = logger;

        var azureConfig = _configuration.AzureBlob;

        if (azureConfig.UseManagedIdentity) {
            var credential = string.IsNullOrEmpty(azureConfig.ManagedIdentityClientId)
                ? new DefaultAzureCredential()
                : new DefaultAzureCredential(new DefaultAzureCredentialOptions {
                    ManagedIdentityClientId = azureConfig.ManagedIdentityClientId
                });

            var blobServiceUri = new Uri($"https://{azureConfig.StorageAccountName}.blob.core.windows.net");
            _blobServiceClient = new BlobServiceClient(blobServiceUri, credential);
        }
        else {
            _blobServiceClient = new BlobServiceClient(azureConfig.ConnectionString);
        }
    }

    public async Task<string> UploadAsync(
        string containerName,
        string fileName,
        Stream content,
        string contentType,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default) {
        try {
            var containerClient = await GetContainerClientAsync(containerName, cancellationToken);
            var blobClient = containerClient.GetBlobClient(fileName);

            var options = new BlobUploadOptions {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType },
                Metadata = metadata ?? [],
                Conditions = new BlobRequestConditions(),
                ProgressHandler = new Progress<long>()
            };

            var response = await blobClient.UploadAsync(content, options, cancellationToken);

            _logger.LogInformation("Successfully uploaded file {FileName} to container {ContainerName}", fileName, containerName);

            return blobClient.Uri.ToString();
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error uploading file {FileName} to container {ContainerName}", fileName, containerName);
            throw;
        }
    }

    public async Task<Stream> DownloadAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default) {
        try {
            var containerClient = await GetContainerClientAsync(containerName, cancellationToken);
            var blobClient = containerClient.GetBlobClient(fileName);

            var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);

            _logger.LogInformation("Successfully downloaded file {FileName} from container {ContainerName}", fileName, containerName);

            return response.Value.Content;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error downloading file {FileName} from container {ContainerName}", fileName, containerName);
            throw;
        }
    }

    public async Task DownloadToFileAsync(
        string containerName,
        string fileName,
        string destinationPath,
        CancellationToken cancellationToken = default) {
        try {
            var containerClient = await GetContainerClientAsync(containerName, cancellationToken);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.DownloadToAsync(destinationPath, cancellationToken);

            _logger.LogInformation("Successfully downloaded file {FileName} to {DestinationPath}", fileName, destinationPath);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error downloading file {FileName} to {DestinationPath}", fileName, destinationPath);
            throw;
        }
    }

    public async Task DeleteAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default) {
        try {
            var containerClient = await GetContainerClientAsync(containerName, cancellationToken);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

            _logger.LogInformation("Successfully deleted file {FileName} from container {ContainerName}", fileName, containerName);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error deleting file {FileName} from container {ContainerName}", fileName, containerName);
            throw;
        }
    }

    public async Task DeleteBatchAsync(
        string containerName,
        IEnumerable<string> fileNames,
        CancellationToken cancellationToken = default) {
        try {
            var containerClient = await GetContainerClientAsync(containerName, cancellationToken);
            var deleteTasks = fileNames.Select(async fileName => {
                var blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
            });

            await Task.WhenAll(deleteTasks);

            _logger.LogInformation("Successfully deleted batch of files from container {ContainerName}", containerName);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error deleting batch of files from container {ContainerName}", containerName);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default) {
        try {
            var containerClient = await GetContainerClientAsync(containerName, cancellationToken);
            var blobClient = containerClient.GetBlobClient(fileName);

            var response = await blobClient.ExistsAsync(cancellationToken);
            return response.Value;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error checking if file {FileName} exists in container {ContainerName}", fileName, containerName);
            throw;
        }
    }

    public async Task<StorageFileMetadata> GetMetadataAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default) {
        try {
            var containerClient = await GetContainerClientAsync(containerName, cancellationToken);
            var blobClient = containerClient.GetBlobClient(fileName);

            var properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
            var props = properties.Value;

            return new StorageFileMetadata {
                FileName = fileName,
                Size = props.ContentLength,
                ContentType = props.ContentType ?? "application/octet-stream",
                ETag = props.ETag?.ToString(),
                LastModified = props.LastModified,
                Metadata = props.Metadata ?? new Dictionary<string, string>()
            };
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error getting metadata for file {FileName} in container {ContainerName}", fileName, containerName);
            throw;
        }
    }

    public async Task<IEnumerable<StorageFileInfo>> ListFilesAsync(
        string containerName,
        string? prefix = null,
        CancellationToken cancellationToken = default) {
        try {
            var containerClient = await GetContainerClientAsync(containerName, cancellationToken);
            var files = new List<StorageFileInfo>();

            await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken)) {
                files.Add(new StorageFileInfo {
                    FileName = blobItem.Name,
                    Size = blobItem.Properties.ContentLength ?? 0,
                    ContentType = blobItem.Properties.ContentType ?? "application/octet-stream",
                    LastModified = blobItem.Properties.LastModified ?? DateTimeOffset.UtcNow,
                    ETag = blobItem.Properties.ETag?.ToString(),
                    IsDirectory = false
                });
            }

            return files;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error listing files in container {ContainerName} with prefix {Prefix}", containerName, prefix);
            throw;
        }
    }

    public async Task<string> GetPresignedUrlAsync(
        string containerName,
        string fileName,
        TimeSpan expiration,
        StoragePermissions permissions = StoragePermissions.Read,
        CancellationToken cancellationToken = default) {
        try {
            var containerClient = await GetContainerClientAsync(containerName, cancellationToken);
            var blobClient = containerClient.GetBlobClient(fileName);

            var sasBuilder = new BlobSasBuilder {
                BlobContainerName = containerName,
                BlobName = fileName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.Add(expiration)
            };

            // Set permissions based on the StoragePermissions enum
            if (permissions.HasFlag(StoragePermissions.Read))
                sasBuilder.SetPermissions(BlobSasPermissions.Read);
            if (permissions.HasFlag(StoragePermissions.Write))
                sasBuilder.SetPermissions(sasBuilder.Permissions | BlobSasPermissions.Write);
            if (permissions.HasFlag(StoragePermissions.Delete))
                sasBuilder.SetPermissions(sasBuilder.Permissions | BlobSasPermissions.Delete);

            var sasUri = blobClient.GenerateSasUri(sasBuilder);

            _logger.LogInformation("Generated presigned URL for file {FileName} in container {ContainerName}", fileName, containerName);

            return sasUri.ToString();
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error generating presigned URL for file {FileName} in container {ContainerName}", fileName, containerName);
            throw;
        }
    }

    public async Task CreateContainerAsync(
        string containerName,
        bool isPublic = false,
        CancellationToken cancellationToken = default) {
        try {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            var accessType = isPublic ? PublicAccessType.Blob : PublicAccessType.None;
            await containerClient.CreateIfNotExistsAsync(accessType, cancellationToken: cancellationToken);

            _logger.LogInformation("Successfully created container {ContainerName}", containerName);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error creating container {ContainerName}", containerName);
            throw;
        }
    }

    public async Task DeleteContainerAsync(
        string containerName,
        CancellationToken cancellationToken = default) {
        try {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

            _logger.LogInformation("Successfully deleted container {ContainerName}", containerName);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error deleting container {ContainerName}", containerName);
            throw;
        }
    }

    public async Task CopyAsync(
        string sourceContainer,
        string sourceFileName,
        string destinationContainer,
        string destinationFileName,
        CancellationToken cancellationToken = default) {
        try {
            var sourceContainerClient = await GetContainerClientAsync(sourceContainer, cancellationToken);
            var sourceBlobClient = sourceContainerClient.GetBlobClient(sourceFileName);

            var destinationContainerClient = await GetContainerClientAsync(destinationContainer, cancellationToken);
            var destinationBlobClient = destinationContainerClient.GetBlobClient(destinationFileName);

            await destinationBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri, cancellationToken: cancellationToken);

            _logger.LogInformation("Successfully copied file from {SourceContainer}/{SourceFileName} to {DestinationContainer}/{DestinationFileName}",
                sourceContainer, sourceFileName, destinationContainer, destinationFileName);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error copying file from {SourceContainer}/{SourceFileName} to {DestinationContainer}/{DestinationFileName}",
                sourceContainer, sourceFileName, destinationContainer, destinationFileName);
            throw;
        }
    }

    public async Task<StorageUsageInfo> GetUsageAsync(
        string containerName,
        CancellationToken cancellationToken = default) {
        try {
            var containerClient = await GetContainerClientAsync(containerName, cancellationToken);

            long totalSize = 0;
            long fileCount = 0;

            await foreach (var blobItem in containerClient.GetBlobsAsync(cancellationToken: cancellationToken)) {
                totalSize += blobItem.Properties.ContentLength ?? 0;
                fileCount++;
            }

            return new StorageUsageInfo {
                TotalSize = totalSize,
                FileCount = fileCount,
                ContainerName = containerName,
                LastUpdated = DateTimeOffset.UtcNow
            };
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error getting usage information for container {ContainerName}", containerName);
            throw;
        }
    }

    private async Task<BlobContainerClient> GetContainerClientAsync(string containerName, CancellationToken cancellationToken = default) {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        return containerClient;
    }
}