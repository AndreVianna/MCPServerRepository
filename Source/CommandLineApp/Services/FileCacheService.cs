using System.Collections.Concurrent;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

using MCPHub.CommandLineApp.Configuration;

namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// File-based cache service implementation with compression and thread safety
/// </summary>
public class FileCacheService : ICacheService {
    private readonly ILogger<FileCacheService> _logger;
    private readonly McpmConfiguration _configuration;
    private readonly string _cacheDirectory;
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _keyLocks = new();
    private readonly SemaphoreSlim _globalLock = new(1, 1);
    private readonly Timer? _maintenanceTimer;
    private readonly CacheMetrics _metrics = new();

    private readonly JsonSerializerOptions _jsonOptions = new() {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    public FileCacheService(ILogger<FileCacheService> logger, McpmConfiguration configuration) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        _cacheDirectory = _configuration.Paths.Cache;
        EnsureCacheDirectoryExists();

        // Start background maintenance if enabled
        if (_configuration.Cache.BackgroundMaintenance) {
            _maintenanceTimer = new Timer(
                async _ => await PerformBackgroundMaintenanceAsync(),
                null,
                _configuration.Cache.MaintenanceInterval,
                _configuration.Cache.MaintenanceInterval);
        }

        _logger.LogInformation("File cache service initialized with directory: {CacheDirectory}", _cacheDirectory);
    }

    public async Task SetAsync<T>(string key, T item, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class {
        if (!_configuration.Cache.Enabled) {
            _logger.LogDebug("Cache disabled, skipping set operation for key: {Key}", key);
            return;
        }

        ValidateKey(key);
        ArgumentNullException.ThrowIfNull(item);

        var keyLock = GetKeyLock(key);
        await keyLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try {
            var cacheEntry = new CacheEntry<T> {
                Key = key,
                Value = item,
                Type = typeof(T).Name,
                CreatedAt = DateTimeOffset.UtcNow,
                LastAccessedAt = DateTimeOffset.UtcNow,
                ExpiresAt = expiration.HasValue ? DateTimeOffset.UtcNow.Add(expiration.Value) : null,
                AccessCount = 0,
            };

            var filePath = GetCacheFilePath(key);
            var data = JsonSerializer.Serialize(cacheEntry, _jsonOptions);
            var bytes = Encoding.UTF8.GetBytes(data);

            if (_configuration.Cache.EnableCompression) {
                bytes = await CompressDataAsync(bytes, cancellationToken).ConfigureAwait(false);
            }

            await File.WriteAllBytesAsync(filePath, bytes, cancellationToken).ConfigureAwait(false);

            CacheMetrics.RecordSet(key, bytes.Length);
            _logger.LogDebug("Cached item with key: {Key}, size: {Size} bytes", key, bytes.Length);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to cache item with key: {Key}", key);
            throw;
        }
        finally {
            keyLock.Release();
        }
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class {
        if (!_configuration.Cache.Enabled) {
            _logger.LogDebug("Cache disabled, returning null for key: {Key}", key);
            return null;
        }

        ValidateKey(key);

        var keyLock = GetKeyLock(key);
        await keyLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try {
            var filePath = GetCacheFilePath(key);
            if (!File.Exists(filePath)) {
                _metrics.RecordMiss(key);
                return null;
            }

            var bytes = await File.ReadAllBytesAsync(filePath, cancellationToken).ConfigureAwait(false);

            if (_configuration.Cache.EnableCompression) {
                bytes = await DecompressDataAsync(bytes, cancellationToken).ConfigureAwait(false);
            }

            var data = Encoding.UTF8.GetString(bytes);
            var cacheEntry = JsonSerializer.Deserialize<CacheEntry<T>>(data, _jsonOptions);

            if (cacheEntry == null) {
                _logger.LogWarning("Failed to deserialize cache entry for key: {Key}", key);
                _metrics.RecordMiss(key);
                return null;
            }

            // Check expiration
            if (cacheEntry.ExpiresAt.HasValue && DateTimeOffset.UtcNow > cacheEntry.ExpiresAt.Value) {
                _logger.LogDebug("Cache entry expired for key: {Key}", key);
                _ = Task.Run(async () => await RemoveAsync(key, CancellationToken.None), CancellationToken.None);
                _metrics.RecordMiss(key);
                return null;
            }

            // Update access information
            cacheEntry.LastAccessedAt = DateTimeOffset.UtcNow;
            cacheEntry.AccessCount++;

            // Write back updated entry asynchronously
            _ = Task.Run(async () => {
                try {
                    var updatedData = JsonSerializer.Serialize(cacheEntry, _jsonOptions);
                    var updatedBytes = Encoding.UTF8.GetBytes(updatedData);

                    if (_configuration.Cache.EnableCompression) {
                        updatedBytes = await CompressDataAsync(updatedBytes, CancellationToken.None).ConfigureAwait(false);
                    }

                    await File.WriteAllBytesAsync(filePath, updatedBytes, CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception ex) {
                    _logger.LogWarning(ex, "Failed to update access information for key: {Key}", key);
                }
            }, CancellationToken.None);

            _metrics.RecordHit(key);
            return cacheEntry.Value;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to retrieve cached item with key: {Key}", key);
            _metrics.RecordMiss(key);
            return null;
        }
        finally {
            keyLock.Release();
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default) {
        if (!_configuration.Cache.Enabled) {
            return false;
        }

        ValidateKey(key);

        var filePath = GetCacheFilePath(key);
        if (!File.Exists(filePath)) {
            return false;
        }

        // Quick check without deserializing - just check expiration from metadata
        try {
            var entryInfo = await GetEntryInfoAsync(key, cancellationToken).ConfigureAwait(false);
            return entryInfo?.IsExpired == false;
        }
        catch {
            return false;
        }
    }

    public async Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default) {
        ValidateKey(key);

        var keyLock = GetKeyLock(key);
        await keyLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try {
            var filePath = GetCacheFilePath(key);
            if (!File.Exists(filePath)) {
                return false;
            }

            File.Delete(filePath);
            CacheMetrics.RecordRemove(key);
            _logger.LogDebug("Removed cache entry for key: {Key}", key);
            return true;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to remove cache entry for key: {Key}", key);
            return false;
        }
        finally {
            keyLock.Release();
        }
    }

    public async Task<int> RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default) {
        var regex = new Regex(ConvertWildcardToRegex(pattern), RegexOptions.IgnoreCase);
        var removed = 0;

        await _globalLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try {
            var files = Directory.GetFiles(_cacheDirectory, "*.cache");

            foreach (var file in files) {
                var key = GetKeyFromFilePath(file);
                if (regex.IsMatch(key)) {
                    if (await RemoveAsync(key, cancellationToken).ConfigureAwait(false)) {
                        removed++;
                    }
                }
            }

            _logger.LogInformation("Removed {Count} cache entries matching pattern: {Pattern}", removed, pattern);
            return removed;
        }
        finally {
            _globalLock.Release();
        }
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default) {
        await _globalLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try {
            var files = Directory.GetFiles(_cacheDirectory, "*.cache");

            foreach (var file in files) {
                try {
                    File.Delete(file);
                }
                catch (Exception ex) {
                    _logger.LogWarning(ex, "Failed to delete cache file: {File}", file);
                }
            }

            _metrics.Reset();
            _logger.LogInformation("Cleared all cache entries ({Count} files)", files.Length);
        }
        finally {
            _globalLock.Release();
        }
    }

    public async Task<CacheStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default) {
        await _globalLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try {
            var statistics = new CacheStatistics {
                HitCount = _metrics.HitCount,
                MissCount = _metrics.MissCount,
                StatisticsResetAt = _metrics.ResetAt,
            };

            var files = Directory.GetFiles(_cacheDirectory, "*.cache");
            statistics.TotalEntries = files.Length;

            long totalSize = 0;
            var expiredCount = 0;
            var entriesByType = new Dictionary<string, int>();

            foreach (var file in files) {
                try {
                    var fileInfo = new FileInfo(file);
                    totalSize += fileInfo.Length;

                    var key = GetKeyFromFilePath(file);
                    var entryInfo = await GetEntryInfoAsync(key, cancellationToken).ConfigureAwait(false);

                    if (entryInfo != null) {
                        if (entryInfo.IsExpired) {
                            expiredCount++;
                        }

                        var type = entryInfo.Type;
                        entriesByType[type] = entriesByType.GetValueOrDefault(type, 0) + 1;
                    }
                }
                catch (Exception ex) {
                    _logger.LogWarning(ex, "Failed to analyze cache file: {File}", file);
                }
            }

            statistics.TotalSizeBytes = totalSize;
            statistics.ExpiredEntries = expiredCount;
            statistics.EntriesByType = entriesByType;

            return statistics;
        }
        finally {
            _globalLock.Release();
        }
    }

    public async Task<CacheEntryInfo?> GetEntryInfoAsync(string key, CancellationToken cancellationToken = default) {
        ValidateKey(key);

        var filePath = GetCacheFilePath(key);
        if (!File.Exists(filePath)) {
            return null;
        }

        try {
            var fileInfo = new FileInfo(filePath);
            var bytes = await File.ReadAllBytesAsync(filePath, cancellationToken).ConfigureAwait(false);

            if (_configuration.Cache.EnableCompression) {
                bytes = await DecompressDataAsync(bytes, cancellationToken).ConfigureAwait(false);
            }

            var data = Encoding.UTF8.GetString(bytes);

            // Parse just the metadata we need
            using var document = JsonDocument.Parse(data);
            var root = document.RootElement;

            return new CacheEntryInfo {
                Key = key,
                Type = root.GetProperty("type").GetString() ?? "Unknown",
                SizeBytes = fileInfo.Length,
                CreatedAt = root.GetProperty("createdAt").GetDateTimeOffset(),
                LastAccessedAt = root.GetProperty("lastAccessedAt").GetDateTimeOffset(),
                ExpiresAt = root.TryGetProperty("expiresAt", out var expiresAtElement) && expiresAtElement.ValueKind != JsonValueKind.Null
                    ? expiresAtElement.GetDateTimeOffset()
                    : null,
                AccessCount = root.GetProperty("accessCount").GetInt32(),
            };
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to get entry info for key: {Key}", key);
            return null;
        }
    }

    public async Task<IEnumerable<string>> GetKeysAsync(string? pattern = null, CancellationToken cancellationToken = default) {
        await _globalLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try {
            var files = Directory.GetFiles(_cacheDirectory, "*.cache");
            var keys = files.Select(GetKeyFromFilePath);

            if (!string.IsNullOrEmpty(pattern)) {
                var regex = new Regex(ConvertWildcardToRegex(pattern), RegexOptions.IgnoreCase);
                keys = keys.Where(key => regex.IsMatch(key));
            }

            return keys.ToList();
        }
        finally {
            _globalLock.Release();
        }
    }

    public async Task<CacheMaintenanceResult> PerformMaintenanceAsync(CancellationToken cancellationToken = default) {
        var startTime = DateTimeOffset.UtcNow;
        var result = new CacheMaintenanceResult();

        await _globalLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try {
            _logger.LogInformation("Starting cache maintenance");

            var files = Directory.GetFiles(_cacheDirectory, "*.cache");
            var expiredFiles = new List<string>();
            long bytesReclaimed = 0;

            // Find expired entries
            foreach (var file in files) {
                try {
                    var key = GetKeyFromFilePath(file);
                    var entryInfo = await GetEntryInfoAsync(key, cancellationToken).ConfigureAwait(false);

                    if (entryInfo?.IsExpired == true) {
                        expiredFiles.Add(file);
                        bytesReclaimed += entryInfo.SizeBytes;
                    }
                }
                catch (Exception ex) {
                    result.Errors.Add($"Failed to check expiration for file {file}: {ex.Message}");
                }
            }

            // Remove expired entries
            foreach (var file in expiredFiles) {
                try {
                    File.Delete(file);
                    result.ExpiredEntriesRemoved++;
                }
                catch (Exception ex) {
                    result.Errors.Add($"Failed to delete expired file {file}: {ex.Message}");
                }
            }

            result.BytesReclaimed = bytesReclaimed;
            result.MaintenanceTime = DateTimeOffset.UtcNow - startTime;

            _logger.LogInformation("Cache maintenance completed: Removed {Count} expired entries, reclaimed {Size}",
                result.ExpiredEntriesRemoved, result.BytesReclaimedFormatted);

            return result;
        }
        finally {
            _globalLock.Release();
        }
    }

    private async Task PerformBackgroundMaintenanceAsync() {
        try {
            var result = await PerformMaintenanceAsync().ConfigureAwait(false);
            if (!result.Success) {
                _logger.LogWarning("Background maintenance completed with errors: {Errors}",
                    string.Join(", ", result.Errors));
            }
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Background maintenance failed");
        }
    }

    private void EnsureCacheDirectoryExists() {
        if (!Directory.Exists(_cacheDirectory)) {
            Directory.CreateDirectory(_cacheDirectory);
        }
    }

    private string GetCacheFilePath(string key) {
        var fileName = GetSafeFileName(key) + ".cache";
        return Path.Combine(_cacheDirectory, fileName);
    }

    private static string GetSafeFileName(string key) {
        // Hash the key to create a safe filename
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static string GetKeyFromFilePath(string filePath) => Path.GetFileNameWithoutExtension(filePath);

    private SemaphoreSlim GetKeyLock(string key) => _keyLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

    private static void ValidateKey(string key) {
        if (string.IsNullOrWhiteSpace(key)) {
            throw new ArgumentException("Cache key cannot be null or empty", nameof(key));
        }

        if (key.Length > 250) {
            throw new ArgumentException("Cache key too long (max 250 characters)", nameof(key));
        }
    }

    private static async Task<byte[]> CompressDataAsync(byte[] data, CancellationToken cancellationToken) {
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionLevel.Optimal)) {
            await gzip.WriteAsync(data, cancellationToken).ConfigureAwait(false);
        }
        return output.ToArray();
    }

    private static async Task<byte[]> DecompressDataAsync(byte[] compressedData, CancellationToken cancellationToken) {
        using var input = new MemoryStream(compressedData);
        using var gzip = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        await gzip.CopyToAsync(output, cancellationToken).ConfigureAwait(false);
        return output.ToArray();
    }

    private static string ConvertWildcardToRegex(string pattern) => "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";

    public void Dispose() {
        _maintenanceTimer?.Dispose();
        _globalLock?.Dispose();

        foreach (var kvp in _keyLocks) {
            kvp.Value.Dispose();
        }
        _keyLocks.Clear();
    }
}

/// <summary>
/// Cache entry wrapper for serialization
/// </summary>
internal class CacheEntry<T> where T : class {
    public string Key { get; set; } = string.Empty;
    public T Value { get; set; } = default!;
    public string Type { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastAccessedAt { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public int AccessCount { get; set; }
}

/// <summary>
/// Internal cache metrics tracking
/// </summary>
internal class CacheMetrics {
    private readonly object _lock = new();

    public long HitCount { get; private set; }
    public long MissCount { get; private set; }
    public DateTimeOffset ResetAt { get; private set; } = DateTimeOffset.UtcNow;

    public void RecordHit(string key) {
        lock (_lock) {
            HitCount++;
        }
    }

    public void RecordMiss(string key) {
        lock (_lock) {
            MissCount++;
        }
    }

    public static void RecordSet(string key, long size) {
        // Could add more detailed metrics here if needed
    }

    public static void RecordRemove(string key) {
        // Could add more detailed metrics here if needed
    }

    public void Reset() {
        lock (_lock) {
            HitCount = 0;
            MissCount = 0;
            ResetAt = DateTimeOffset.UtcNow;
        }
    }
}