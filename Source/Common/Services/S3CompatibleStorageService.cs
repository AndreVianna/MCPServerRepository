using System.Reactive.Linq;

using Common.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace Common.Services;

/// <summary>
/// S3-compatible storage service implementation (MinIO, etc.)
/// </summary>
public class S3CompatibleStorageService : IStorageService {
    private readonly IMinioClient _minioClient;
    private readonly StorageConfiguration _configuration;
    private readonly ILogger<S3CompatibleStorageService> _logger;

    public S3CompatibleStorageService(
        IOptions<StorageConfiguration> configuration,
        ILogger<S3CompatibleStorageService> logger) {
        _configuration = configuration.Value;
        _logger = logger;

        var s3Config = _configuration.S3Compatible;

        _minioClient = new MinioClient()
            .WithEndpoint(s3Config.ServiceUrl)
            .WithCredentials(s3Config.AccessKey, s3Config.SecretKey)
            .WithSSL(s3Config.UseHttps)
            .WithRegion(s3Config.Region)
            .Build();
    }

    public async Task<string> UploadAsync(
        string containerName,
        string fileName,
        Stream content,
        string contentType,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default) {
        try {
            await EnsureBucketExistsAsync(containerName, cancellationToken);

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(containerName)
                .WithObject(fileName)
                .WithStreamData(content)
                .WithObjectSize(content.Length)
                .WithContentType(contentType);

            if (metadata != null && metadata.Count > 0) {
                putObjectArgs = putObjectArgs.WithHeaders(metadata);
            }

            await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

            _logger.LogInformation("Successfully uploaded file {FileName} to bucket {BucketName}", fileName, containerName);

            var protocol = _configuration.S3Compatible.UseHttps ? "https" : "http";
            return $"{protocol}://{_configuration.S3Compatible.ServiceUrl}/{containerName}/{fileName}";
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error uploading file {FileName} to bucket {BucketName}", fileName, containerName);
            throw;
        }
    }

    public async Task<Stream> DownloadAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default) {
        try {
            var memoryStream = new MemoryStream();

            var getObjectArgs = new GetObjectArgs()
                .WithBucket(containerName)
                .WithObject(fileName)
                .WithCallbackStream(stream => stream.CopyTo(memoryStream));

            await _minioClient.GetObjectAsync(getObjectArgs, cancellationToken);

            memoryStream.Position = 0;

            _logger.LogInformation("Successfully downloaded file {FileName} from bucket {BucketName}", fileName, containerName);

            return memoryStream;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error downloading file {FileName} from bucket {BucketName}", fileName, containerName);
            throw;
        }
    }

    public async Task DownloadToFileAsync(
        string containerName,
        string fileName,
        string destinationPath,
        CancellationToken cancellationToken = default) {
        try {
            var getObjectArgs = new GetObjectArgs()
                .WithBucket(containerName)
                .WithObject(fileName)
                .WithFile(destinationPath);

            await _minioClient.GetObjectAsync(getObjectArgs, cancellationToken);

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
            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(containerName)
                .WithObject(fileName);

            await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);

            _logger.LogInformation("Successfully deleted file {FileName} from bucket {BucketName}", fileName, containerName);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error deleting file {FileName} from bucket {BucketName}", fileName, containerName);
            throw;
        }
    }

    public async Task DeleteBatchAsync(
        string containerName,
        IEnumerable<string> fileNames,
        CancellationToken cancellationToken = default) {
        try {
            var objectNames = fileNames.ToList();

            if (objectNames.Count == 0)
                return;

            var removeObjectsArgs = new RemoveObjectsArgs()
                .WithBucket(containerName)
                .WithObjects(objectNames);

            var errors = new List<DeleteError>();

            await foreach (var result in _minioClient.RemoveObjectsAsync(removeObjectsArgs, cancellationToken)) {
                if (result.Exception != null) {
                    errors.Add(new DeleteError {
                        Key = result.Key,
                        Code = result.Exception.GetType().Name,
                        Message = result.Exception.Message
                    });
                }
            }

            if (errors.Count > 0) {
                _logger.LogWarning("Some files failed to delete: {Errors}", string.Join(", ", errors.Select(e => $"{e.Key}: {e.Message}")));
            }

            _logger.LogInformation("Successfully deleted batch of files from bucket {BucketName}", containerName);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error deleting batch of files from bucket {BucketName}", containerName);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default) {
        try {
            var statObjectArgs = new StatObjectArgs()
                .WithBucket(containerName)
                .WithObject(fileName);

            await _minioClient.StatObjectAsync(statObjectArgs, cancellationToken);
            return true;
        }
        catch (ObjectNotFoundException) {
            return false;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error checking if file {FileName} exists in bucket {BucketName}", fileName, containerName);
            throw;
        }
    }

    public async Task<StorageFileMetadata> GetMetadataAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default) {
        try {
            var statObjectArgs = new StatObjectArgs()
                .WithBucket(containerName)
                .WithObject(fileName);

            var objectStat = await _minioClient.StatObjectAsync(statObjectArgs, cancellationToken);

            return new StorageFileMetadata {
                FileName = fileName,
                Size = objectStat.Size,
                ContentType = objectStat.ContentType ?? "application/octet-stream",
                ETag = objectStat.ETag,
                LastModified = objectStat.LastModified,
                Metadata = objectStat.MetaData ?? []
            };
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error getting metadata for file {FileName} in bucket {BucketName}", fileName, containerName);
            throw;
        }
    }

    public async Task<IEnumerable<StorageFileInfo>> ListFilesAsync(
        string containerName,
        string? prefix = null,
        CancellationToken cancellationToken = default) {
        try {
            var files = new List<StorageFileInfo>();

            var listObjectsArgs = new ListObjectsArgs()
                .WithBucket(containerName)
                .WithPrefix(prefix)
                .WithRecursive(true);

            await foreach (var item in _minioClient.ListObjectsEnumAsync(listObjectsArgs, cancellationToken)) {
                files.Add(new StorageFileInfo {
                    FileName = item.Key,
                    Size = item.Size,
                    ContentType = "application/octet-stream", // MinIO doesn't return content type in list
                    LastModified = item.LastModified,
                    ETag = item.ETag,
                    IsDirectory = item.IsDir
                });
            }

            return files;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error listing files in bucket {BucketName} with prefix {Prefix}", containerName, prefix);
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
            var expirationSeconds = (int)expiration.TotalSeconds;

            string presignedUrl;

            if (permissions.HasFlag(StoragePermissions.Write)) {
                var presignedPutObjectArgs = new PresignedPutObjectArgs()
                    .WithBucket(containerName)
                    .WithObject(fileName)
                    .WithExpiry(expirationSeconds);

                presignedUrl = await _minioClient.PresignedPutObjectAsync(presignedPutObjectArgs);
            }
            else {
                var presignedGetObjectArgs = new PresignedGetObjectArgs()
                    .WithBucket(containerName)
                    .WithObject(fileName)
                    .WithExpiry(expirationSeconds);

                presignedUrl = await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);
            }

            _logger.LogInformation("Generated presigned URL for file {FileName} in bucket {BucketName}", fileName, containerName);

            return presignedUrl;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error generating presigned URL for file {FileName} in bucket {BucketName}", fileName, containerName);
            throw;
        }
    }

    public async Task CreateContainerAsync(
        string containerName,
        bool isPublic = false,
        CancellationToken cancellationToken = default) {
        try {
            var bucketExistsArgs = new BucketExistsArgs()
                .WithBucket(containerName);

            var bucketExists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);

            if (!bucketExists) {
                var makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(containerName);

                await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);

                if (isPublic) {
                    var policyJson = GetPublicReadPolicy(containerName);

                    var setBucketPolicyArgs = new SetBucketPolicyArgs()
                        .WithBucket(containerName)
                        .WithPolicy(policyJson);

                    await _minioClient.SetBucketPolicyAsync(setBucketPolicyArgs, cancellationToken);
                }
            }

            _logger.LogInformation("Successfully created bucket {BucketName}", containerName);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error creating bucket {BucketName}", containerName);
            throw;
        }
    }

    public async Task DeleteContainerAsync(
        string containerName,
        CancellationToken cancellationToken = default) {
        try {
            // First, delete all objects in the bucket
            var listObjectsArgs = new ListObjectsArgs()
                .WithBucket(containerName)
                .WithRecursive(true);

            var objectsToDelete = new List<string>();

            await foreach (var item in _minioClient.ListObjectsEnumAsync(listObjectsArgs, cancellationToken)) {
                objectsToDelete.Add(item.Key);
            }

            if (objectsToDelete.Count > 0) {
                var removeObjectsArgs = new RemoveObjectsArgs()
                    .WithBucket(containerName)
                    .WithObjects(objectsToDelete);

                await foreach (var result in _minioClient.RemoveObjectsAsync(removeObjectsArgs, cancellationToken)) {
                    if (result.Exception != null) {
                        _logger.LogWarning("Failed to delete object {Key}: {Error}", result.Key, result.Exception.Message);
                    }
                }
            }

            // Then delete the bucket
            var removeBucketArgs = new RemoveBucketArgs()
                .WithBucket(containerName);

            await _minioClient.RemoveBucketAsync(removeBucketArgs, cancellationToken);

            _logger.LogInformation("Successfully deleted bucket {BucketName}", containerName);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error deleting bucket {BucketName}", containerName);
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
            await EnsureBucketExistsAsync(destinationContainer, cancellationToken);

            var copySourceObjectArgs = new CopySourceObjectArgs()
                .WithBucket(sourceContainer)
                .WithObject(sourceFileName);

            var copyObjectArgs = new CopyObjectArgs()
                .WithBucket(destinationContainer)
                .WithObject(destinationFileName)
                .WithCopyObjectSource(copySourceObjectArgs);

            await _minioClient.CopyObjectAsync(copyObjectArgs, cancellationToken);

            _logger.LogInformation("Successfully copied file from {SourceBucket}/{SourceKey} to {DestinationBucket}/{DestinationKey}",
                sourceContainer, sourceFileName, destinationContainer, destinationFileName);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error copying file from {SourceBucket}/{SourceKey} to {DestinationBucket}/{DestinationKey}",
                sourceContainer, sourceFileName, destinationContainer, destinationFileName);
            throw;
        }
    }

    public async Task<StorageUsageInfo> GetUsageAsync(
        string containerName,
        CancellationToken cancellationToken = default) {
        try {
            long totalSize = 0;
            long fileCount = 0;

            var listObjectsArgs = new ListObjectsArgs()
                .WithBucket(containerName)
                .WithRecursive(true);

            await foreach (var item in _minioClient.ListObjectsEnumAsync(listObjectsArgs, cancellationToken)) {
                totalSize += item.Size;
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
            _logger.LogError(ex, "Error getting usage information for bucket {BucketName}", containerName);
            throw;
        }
    }

    private async Task EnsureBucketExistsAsync(string bucketName, CancellationToken cancellationToken) {
        var bucketExistsArgs = new BucketExistsArgs()
            .WithBucket(bucketName);

        var bucketExists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);

        if (!bucketExists) {
            await CreateContainerAsync(bucketName, false, cancellationToken);
        }
    }

    private static string GetPublicReadPolicy(string bucketName) => $$"""
        {
            "Version": "2012-10-17",
            "Statement": [
                {
                    "Effect": "Allow",
                    "Principal": {
                        "AWS": ["*"]
                    },
                    "Action": [
                        "s3:GetObject"
                    ],
                    "Resource": [
                        "arn:aws:s3:::{{bucketName}}/*"
                    ]
                }
            ]
        }
        """;
}

/// <summary>
/// Represents a delete error from batch operations
/// </summary>
public class DeleteError {
    public string Key { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}