using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Common.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Common.Services;

/// <summary>
/// AWS S3 implementation of IStorageService
/// </summary>
public class AwsS3StorageService : IStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly StorageConfiguration _configuration;
    private readonly ILogger<AwsS3StorageService> _logger;

    public AwsS3StorageService(
        IOptions<StorageConfiguration> configuration,
        ILogger<AwsS3StorageService> logger)
    {
        _configuration = configuration.Value;
        _logger = logger;

        var awsConfig = _configuration.AwsS3;
        var s3Config = new AmazonS3Config
        {
            RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsConfig.Region),
            MaxErrorRetry = awsConfig.MaxRetryAttempts,
            Timeout = TimeSpan.FromSeconds(30)
        };

        if (awsConfig.UseInstanceProfile)
        {
            _s3Client = new AmazonS3Client(s3Config);
        }
        else
        {
            var credentials = string.IsNullOrEmpty(awsConfig.SessionToken)
                ? new BasicAWSCredentials(awsConfig.AccessKeyId, awsConfig.SecretAccessKey)
                : new SessionAWSCredentials(awsConfig.AccessKeyId, awsConfig.SecretAccessKey, awsConfig.SessionToken);

            _s3Client = new AmazonS3Client(credentials, s3Config);
        }
    }

    public async Task<string> UploadAsync(
        string containerName,
        string fileName,
        Stream content,
        string contentType,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureBucketExistsAsync(containerName, cancellationToken);

            var request = new PutObjectRequest
            {
                BucketName = containerName,
                Key = fileName,
                InputStream = content,
                ContentType = contentType,
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
                CannedACL = S3CannedACL.Private
            };

            if (metadata != null)
            {
                foreach (var kvp in metadata)
                {
                    request.Metadata.Add(kvp.Key, kvp.Value);
                }
            }

            var response = await _s3Client.PutObjectAsync(request, cancellationToken);
            
            _logger.LogInformation("Successfully uploaded file {FileName} to bucket {BucketName}", fileName, containerName);
            
            return $"https://{containerName}.s3.{_configuration.AwsS3.Region}.amazonaws.com/{fileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName} to bucket {BucketName}", fileName, containerName);
            throw;
        }
    }

    public async Task<Stream> DownloadAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = containerName,
                Key = fileName
            };

            var response = await _s3Client.GetObjectAsync(request, cancellationToken);
            
            _logger.LogInformation("Successfully downloaded file {FileName} from bucket {BucketName}", fileName, containerName);
            
            return response.ResponseStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file {FileName} from bucket {BucketName}", fileName, containerName);
            throw;
        }
    }

    public async Task DownloadToFileAsync(
        string containerName,
        string fileName,
        string destinationPath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var stream = await DownloadAsync(containerName, fileName, cancellationToken);
            using var fileStream = File.Create(destinationPath);
            await stream.CopyToAsync(fileStream, cancellationToken);
            
            _logger.LogInformation("Successfully downloaded file {FileName} to {DestinationPath}", fileName, destinationPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file {FileName} to {DestinationPath}", fileName, destinationPath);
            throw;
        }
    }

    public async Task DeleteAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = containerName,
                Key = fileName
            };

            await _s3Client.DeleteObjectAsync(request, cancellationToken);
            
            _logger.LogInformation("Successfully deleted file {FileName} from bucket {BucketName}", fileName, containerName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FileName} from bucket {BucketName}", fileName, containerName);
            throw;
        }
    }

    public async Task DeleteBatchAsync(
        string containerName,
        IEnumerable<string> fileNames,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var keys = fileNames.Select(fn => new KeyVersion { Key = fn }).ToList();
            
            if (keys.Count == 0) return;

            var request = new DeleteObjectsRequest
            {
                BucketName = containerName,
                Objects = keys
            };

            await _s3Client.DeleteObjectsAsync(request, cancellationToken);
            
            _logger.LogInformation("Successfully deleted batch of files from bucket {BucketName}", containerName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting batch of files from bucket {BucketName}", containerName);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = containerName,
                Key = fileName
            };

            await _s3Client.GetObjectMetadataAsync(request, cancellationToken);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if file {FileName} exists in bucket {BucketName}", fileName, containerName);
            throw;
        }
    }

    public async Task<StorageFileMetadata> GetMetadataAsync(
        string containerName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = containerName,
                Key = fileName
            };

            var response = await _s3Client.GetObjectMetadataAsync(request, cancellationToken);

            return new StorageFileMetadata
            {
                FileName = fileName,
                Size = response.ContentLength,
                ContentType = response.Headers.ContentType ?? "application/octet-stream",
                ETag = response.ETag,
                LastModified = response.LastModified,
                Metadata = response.Metadata.MetadataCollection.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting metadata for file {FileName} in bucket {BucketName}", fileName, containerName);
            throw;
        }
    }

    public async Task<IEnumerable<StorageFileInfo>> ListFilesAsync(
        string containerName,
        string? prefix = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var files = new List<StorageFileInfo>();
            var request = new ListObjectsV2Request
            {
                BucketName = containerName,
                Prefix = prefix,
                MaxKeys = 1000
            };

            ListObjectsV2Response response;
            do
            {
                response = await _s3Client.ListObjectsV2Async(request, cancellationToken);
                
                foreach (var obj in response.S3Objects)
                {
                    files.Add(new StorageFileInfo
                    {
                        FileName = obj.Key,
                        Size = obj.Size,
                        ContentType = "application/octet-stream", // S3 doesn't return content type in list
                        LastModified = obj.LastModified,
                        ETag = obj.ETag,
                        IsDirectory = obj.Key.EndsWith("/")
                    });
                }

                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);

            return files;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing files in bucket {BucketName} with prefix {Prefix}", containerName, prefix);
            throw;
        }
    }

    public async Task<string> GetPresignedUrlAsync(
        string containerName,
        string fileName,
        TimeSpan expiration,
        StoragePermissions permissions = StoragePermissions.Read,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var verb = permissions.HasFlag(StoragePermissions.Write) ? HttpVerb.PUT : HttpVerb.GET;
            
            var request = new GetPreSignedUrlRequest
            {
                BucketName = containerName,
                Key = fileName,
                Verb = verb,
                Expires = DateTime.UtcNow.Add(expiration)
            };

            var presignedUrl = await _s3Client.GetPreSignedURLAsync(request);
            
            _logger.LogInformation("Generated presigned URL for file {FileName} in bucket {BucketName}", fileName, containerName);
            
            return presignedUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating presigned URL for file {FileName} in bucket {BucketName}", fileName, containerName);
            throw;
        }
    }

    public async Task CreateContainerAsync(
        string containerName,
        bool isPublic = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, containerName);
            
            if (!bucketExists)
            {
                var request = new PutBucketRequest
                {
                    BucketName = containerName,
                    UseClientRegion = true
                };

                await _s3Client.PutBucketAsync(request, cancellationToken);

                if (isPublic)
                {
                    var policyRequest = new PutBucketPolicyRequest
                    {
                        BucketName = containerName,
                        Policy = GetPublicReadPolicy(containerName)
                    };

                    await _s3Client.PutBucketPolicyAsync(policyRequest, cancellationToken);
                }
            }
            
            _logger.LogInformation("Successfully created bucket {BucketName}", containerName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating bucket {BucketName}", containerName);
            throw;
        }
    }

    public async Task DeleteContainerAsync(
        string containerName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // First, delete all objects in the bucket
            var listRequest = new ListObjectsV2Request { BucketName = containerName };
            ListObjectsV2Response listResponse;
            
            do
            {
                listResponse = await _s3Client.ListObjectsV2Async(listRequest, cancellationToken);
                
                if (listResponse.S3Objects.Count > 0)
                {
                    var deleteRequest = new DeleteObjectsRequest
                    {
                        BucketName = containerName,
                        Objects = listResponse.S3Objects.Select(obj => new KeyVersion { Key = obj.Key }).ToList()
                    };

                    await _s3Client.DeleteObjectsAsync(deleteRequest, cancellationToken);
                }

                listRequest.ContinuationToken = listResponse.NextContinuationToken;
            } while (listResponse.IsTruncated);

            // Then delete the bucket
            var deleteBucketRequest = new DeleteBucketRequest { BucketName = containerName };
            await _s3Client.DeleteBucketAsync(deleteBucketRequest, cancellationToken);
            
            _logger.LogInformation("Successfully deleted bucket {BucketName}", containerName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting bucket {BucketName}", containerName);
            throw;
        }
    }

    public async Task CopyAsync(
        string sourceContainer,
        string sourceFileName,
        string destinationContainer,
        string destinationFileName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureBucketExistsAsync(destinationContainer, cancellationToken);

            var request = new CopyObjectRequest
            {
                SourceBucket = sourceContainer,
                SourceKey = sourceFileName,
                DestinationBucket = destinationContainer,
                DestinationKey = destinationFileName,
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
            };

            await _s3Client.CopyObjectAsync(request, cancellationToken);
            
            _logger.LogInformation("Successfully copied file from {SourceBucket}/{SourceKey} to {DestinationBucket}/{DestinationKey}", 
                sourceContainer, sourceFileName, destinationContainer, destinationFileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error copying file from {SourceBucket}/{SourceKey} to {DestinationBucket}/{DestinationKey}", 
                sourceContainer, sourceFileName, destinationContainer, destinationFileName);
            throw;
        }
    }

    public async Task<StorageUsageInfo> GetUsageAsync(
        string containerName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            long totalSize = 0;
            long fileCount = 0;
            
            var request = new ListObjectsV2Request
            {
                BucketName = containerName,
                MaxKeys = 1000
            };

            ListObjectsV2Response response;
            do
            {
                response = await _s3Client.ListObjectsV2Async(request, cancellationToken);
                
                foreach (var obj in response.S3Objects)
                {
                    totalSize += obj.Size;
                    fileCount++;
                }

                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);

            return new StorageUsageInfo
            {
                TotalSize = totalSize,
                FileCount = fileCount,
                ContainerName = containerName,
                LastUpdated = DateTimeOffset.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting usage information for bucket {BucketName}", containerName);
            throw;
        }
    }

    private async Task EnsureBucketExistsAsync(string bucketName, CancellationToken cancellationToken)
    {
        var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
        
        if (!bucketExists)
        {
            await CreateContainerAsync(bucketName, false, cancellationToken);
        }
    }

    private string GetPublicReadPolicy(string bucketName)
    {
        return $$"""
        {
            "Version": "2012-10-17",
            "Statement": [
                {
                    "Sid": "PublicReadGetObject",
                    "Effect": "Allow",
                    "Principal": "*",
                    "Action": "s3:GetObject",
                    "Resource": "arn:aws:s3:::{{bucketName}}/*"
                }
            ]
        }
        """;
    }

    public void Dispose()
    {
        _s3Client?.Dispose();
    }
}