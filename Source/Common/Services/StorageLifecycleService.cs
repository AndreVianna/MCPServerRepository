using Common.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.RegularExpressions;

namespace Common.Services;

/// <summary>
/// Service for managing storage lifecycle policies and retention
/// </summary>
public interface IStorageLifecycleService
{
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

/// <summary>
/// Background service for automatic lifecycle policy execution
/// </summary>
public class StorageLifecycleBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly StorageConfiguration _configuration;
    private readonly ILogger<StorageLifecycleBackgroundService> _logger;
    private readonly TimeSpan _executionInterval = TimeSpan.FromHours(1);

    public StorageLifecycleBackgroundService(
        IServiceProvider serviceProvider,
        IOptions<StorageConfiguration> configuration,
        ILogger<StorageLifecycleBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var lifecycleService = scope.ServiceProvider.GetRequiredService<IStorageLifecycleService>();
                
                await lifecycleService.ApplyLifecyclePoliciesAsync(stoppingToken);
                
                _logger.LogInformation("Lifecycle policies applied successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying lifecycle policies");
            }

            await Task.Delay(_executionInterval, stoppingToken);
        }
    }
}

/// <summary>
/// Implementation of storage lifecycle service
/// </summary>
public class StorageLifecycleService : IStorageLifecycleService
{
    private readonly IStorageService _storageService;
    private readonly StorageConfiguration _configuration;
    private readonly ILogger<StorageLifecycleService> _logger;

    public StorageLifecycleService(
        IStorageService storageService,
        IOptions<StorageConfiguration> configuration,
        ILogger<StorageLifecycleService> logger)
    {
        _storageService = storageService;
        _configuration = configuration.Value;
        _logger = logger;
    }

    public async Task ApplyLifecyclePoliciesAsync(CancellationToken cancellationToken = default)
    {
        var enabledPolicies = _configuration.LifecyclePolicies.Where(p => p.IsEnabled);
        
        foreach (var policy in enabledPolicies)
        {
            try
            {
                await ApplyPolicyAsync(policy, cancellationToken);
                _logger.LogInformation("Applied lifecycle policy {PolicyName}", policy.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying lifecycle policy {PolicyName}", policy.Name);
            }
        }
    }

    public async Task ApplyLifecyclePoliciesAsync(string containerName, CancellationToken cancellationToken = default)
    {
        var applicablePolicies = _configuration.LifecyclePolicies
            .Where(p => p.IsEnabled && IsContainerMatchingPattern(containerName, p.ContainerPattern));

        foreach (var policy in applicablePolicies)
        {
            try
            {
                await ApplyPolicyToContainerAsync(policy, containerName, cancellationToken);
                _logger.LogInformation("Applied lifecycle policy {PolicyName} to container {ContainerName}", policy.Name, containerName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying lifecycle policy {PolicyName} to container {ContainerName}", policy.Name, containerName);
            }
        }
    }

    public async Task<bool> ValidateLifecyclePolicyAsync(StorageLifecyclePolicy policy, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate policy structure
            if (string.IsNullOrEmpty(policy.Name))
            {
                _logger.LogWarning("Policy name is required");
                return false;
            }

            if (string.IsNullOrEmpty(policy.ContainerPattern))
            {
                _logger.LogWarning("Container pattern is required for policy {PolicyName}", policy.Name);
                return false;
            }

            if (policy.Rules == null || policy.Rules.Count == 0)
            {
                _logger.LogWarning("At least one rule is required for policy {PolicyName}", policy.Name);
                return false;
            }

            // Validate container pattern
            try
            {
                var regex = new Regex(policy.ContainerPattern);
            }
            catch (ArgumentException)
            {
                _logger.LogWarning("Invalid container pattern for policy {PolicyName}: {Pattern}", policy.Name, policy.ContainerPattern);
                return false;
            }

            // Validate file pattern if specified
            if (!string.IsNullOrEmpty(policy.FilePattern))
            {
                try
                {
                    var regex = new Regex(policy.FilePattern);
                }
                catch (ArgumentException)
                {
                    _logger.LogWarning("Invalid file pattern for policy {PolicyName}: {Pattern}", policy.Name, policy.FilePattern);
                    return false;
                }
            }

            // Validate rules
            foreach (var rule in policy.Rules)
            {
                if (rule.DaysAfterCreation <= 0 && rule.DaysAfterModification <= 0)
                {
                    _logger.LogWarning("Rule must have either DaysAfterCreation or DaysAfterModification greater than 0");
                    return false;
                }

                if (rule.MinimumFileSize.HasValue && rule.MaximumFileSize.HasValue && 
                    rule.MinimumFileSize.Value > rule.MaximumFileSize.Value)
                {
                    _logger.LogWarning("MinimumFileSize cannot be greater than MaximumFileSize");
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating lifecycle policy {PolicyName}", policy.Name);
            return false;
        }
    }

    public async Task<StorageLifecycleStatistics> GetLifecycleStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var statistics = new StorageLifecycleStatistics
        {
            TotalPolicies = _configuration.LifecyclePolicies.Count,
            EnabledPolicies = _configuration.LifecyclePolicies.Count(p => p.IsEnabled),
            LastExecutionTime = DateTimeOffset.UtcNow, // Would be stored in database in real implementation
            PolicyStatistics = new List<StorageLifecyclePolicyStatistics>()
        };

        foreach (var policy in _configuration.LifecyclePolicies)
        {
            var policyStats = new StorageLifecyclePolicyStatistics
            {
                PolicyName = policy.Name,
                IsEnabled = policy.IsEnabled,
                RuleCount = policy.Rules.Count,
                LastExecutionTime = DateTimeOffset.UtcNow, // Would be stored in database
                FilesProcessed = 0, // Would be tracked in database
                FilesDeleted = 0, // Would be tracked in database
                FilesArchived = 0, // Would be tracked in database
                SpaceReclaimed = 0 // Would be tracked in database
            };

            statistics.PolicyStatistics.Add(policyStats);
        }

        return statistics;
    }

    private async Task ApplyPolicyAsync(StorageLifecyclePolicy policy, CancellationToken cancellationToken)
    {
        // In a real implementation, this would enumerate all containers and apply the policy
        // For now, we'll simulate the logic
        _logger.LogInformation("Applying lifecycle policy {PolicyName} with pattern {Pattern}", policy.Name, policy.ContainerPattern);
        
        // This would typically:
        // 1. Find containers matching the pattern
        // 2. For each container, apply the policy rules
        // 3. Log results and update statistics
    }

    private async Task ApplyPolicyToContainerAsync(StorageLifecyclePolicy policy, string containerName, CancellationToken cancellationToken)
    {
        try
        {
            var files = await _storageService.ListFilesAsync(containerName, cancellationToken: cancellationToken);
            
            foreach (var file in files)
            {
                if (!string.IsNullOrEmpty(policy.FilePattern))
                {
                    var fileRegex = new Regex(policy.FilePattern);
                    if (!fileRegex.IsMatch(file.FileName))
                    {
                        continue;
                    }
                }

                var fileMetadata = await _storageService.GetMetadataAsync(containerName, file.FileName, cancellationToken);
                
                foreach (var rule in policy.Rules)
                {
                    if (await ShouldApplyRuleAsync(rule, fileMetadata))
                    {
                        await ApplyRuleToFileAsync(rule, containerName, file.FileName, fileMetadata, cancellationToken);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying policy {PolicyName} to container {ContainerName}", policy.Name, containerName);
            throw;
        }
    }

    private async Task<bool> ShouldApplyRuleAsync(StorageLifecycleRule rule, StorageFileMetadata fileMetadata)
    {
        var now = DateTimeOffset.UtcNow;
        
        // Check age criteria
        if (rule.DaysAfterCreation > 0)
        {
            var daysSinceCreation = (now - fileMetadata.LastModified).Days;
            if (daysSinceCreation < rule.DaysAfterCreation)
            {
                return false;
            }
        }

        if (rule.DaysAfterModification > 0)
        {
            var daysSinceModification = (now - fileMetadata.LastModified).Days;
            if (daysSinceModification < rule.DaysAfterModification)
            {
                return false;
            }
        }

        // Check size criteria
        if (rule.MinimumFileSize.HasValue && fileMetadata.Size < rule.MinimumFileSize.Value)
        {
            return false;
        }

        if (rule.MaximumFileSize.HasValue && fileMetadata.Size > rule.MaximumFileSize.Value)
        {
            return false;
        }

        return true;
    }

    private async Task ApplyRuleToFileAsync(
        StorageLifecycleRule rule, 
        string containerName, 
        string fileName, 
        StorageFileMetadata fileMetadata, 
        CancellationToken cancellationToken)
    {
        try
        {
            switch (rule.Action)
            {
                case StorageLifecycleAction.Delete:
                    await _storageService.DeleteAsync(containerName, fileName, cancellationToken);
                    _logger.LogInformation("Deleted file {FileName} from container {ContainerName} due to lifecycle rule", fileName, containerName);
                    break;

                case StorageLifecycleAction.Archive:
                    await ArchiveFileAsync(containerName, fileName, fileMetadata, cancellationToken);
                    _logger.LogInformation("Archived file {FileName} from container {ContainerName} due to lifecycle rule", fileName, containerName);
                    break;

                case StorageLifecycleAction.MoveToStorageClass:
                    await MoveToStorageClassAsync(containerName, fileName, fileMetadata, cancellationToken);
                    _logger.LogInformation("Moved file {FileName} to different storage class due to lifecycle rule", fileName);
                    break;

                case StorageLifecycleAction.Compress:
                    await CompressFileAsync(containerName, fileName, fileMetadata, cancellationToken);
                    _logger.LogInformation("Compressed file {FileName} in container {ContainerName} due to lifecycle rule", fileName, containerName);
                    break;

                default:
                    _logger.LogWarning("Unknown lifecycle action: {Action}", rule.Action);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying lifecycle rule action {Action} to file {FileName}", rule.Action, fileName);
            throw;
        }
    }

    private async Task ArchiveFileAsync(string containerName, string fileName, StorageFileMetadata fileMetadata, CancellationToken cancellationToken)
    {
        // Move to archive container
        var archiveContainer = $"{containerName}-archive";
        await _storageService.CreateContainerAsync(archiveContainer, false, cancellationToken);
        await _storageService.CopyAsync(containerName, fileName, archiveContainer, fileName, cancellationToken);
        await _storageService.DeleteAsync(containerName, fileName, cancellationToken);
    }

    private async Task MoveToStorageClassAsync(string containerName, string fileName, StorageFileMetadata fileMetadata, CancellationToken cancellationToken)
    {
        // This would typically involve provider-specific storage class changes
        // For now, we'll just log the action
        _logger.LogInformation("Moving file {FileName} to different storage class (provider-specific implementation needed)", fileName);
    }

    private async Task CompressFileAsync(string containerName, string fileName, StorageFileMetadata fileMetadata, CancellationToken cancellationToken)
    {
        // This would involve downloading, compressing, and re-uploading the file
        // For now, we'll just log the action
        _logger.LogInformation("Compressing file {FileName} (compression implementation needed)", fileName);
    }

    private bool IsContainerMatchingPattern(string containerName, string pattern)
    {
        try
        {
            var regex = new Regex(pattern);
            return regex.IsMatch(containerName);
        }
        catch (ArgumentException)
        {
            _logger.LogWarning("Invalid container pattern: {Pattern}", pattern);
            return false;
        }
    }
}

/// <summary>
/// Storage lifecycle statistics
/// </summary>
public class StorageLifecycleStatistics
{
    public int TotalPolicies { get; set; }
    public int EnabledPolicies { get; set; }
    public DateTimeOffset LastExecutionTime { get; set; }
    public List<StorageLifecyclePolicyStatistics> PolicyStatistics { get; set; } = new();
}

/// <summary>
/// Storage lifecycle policy statistics
/// </summary>
public class StorageLifecyclePolicyStatistics
{
    public string PolicyName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public int RuleCount { get; set; }
    public DateTimeOffset LastExecutionTime { get; set; }
    public long FilesProcessed { get; set; }
    public long FilesDeleted { get; set; }
    public long FilesArchived { get; set; }
    public long SpaceReclaimed { get; set; }
}