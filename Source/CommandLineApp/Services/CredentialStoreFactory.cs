namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Factory for creating platform-appropriate credential stores
/// </summary>
public class CredentialStoreFactory(IServiceProvider serviceProvider, ILogger<CredentialStoreFactory> logger) : ICredentialStoreFactory {
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly ILogger<CredentialStoreFactory> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<ICredentialStore> CreateCredentialStoreAsync(CancellationToken cancellationToken = default) {
        var availableStores = await GetAvailableStoresAsync(cancellationToken).ConfigureAwait(false);
        var preferredStore = availableStores.FirstOrDefault();

        if (preferredStore == null) {
            _logger.LogWarning("No credential stores are available, using fallback store");
            return _serviceProvider.GetRequiredService<FallbackCredentialStore>();
        }

        _logger.LogDebug("Selected credential store: {StoreMechanism}", preferredStore.StorageMechanism);
        return preferredStore;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ICredentialStore>> GetAvailableStoresAsync(CancellationToken cancellationToken = default) {
        var stores = new List<ICredentialStore>();

        // Platform-specific stores
        if (OperatingSystem.IsWindows()) {
            var windowsStore = _serviceProvider.GetRequiredService<WindowsCredentialStore>();
            if (await windowsStore.IsAvailableAsync(cancellationToken).ConfigureAwait(false)) {
                stores.Add(windowsStore);
            }
        }
        else if (OperatingSystem.IsMacOS()) {
            var macStore = _serviceProvider.GetRequiredService<MacOSKeychainStore>();
            if (await macStore.IsAvailableAsync(cancellationToken).ConfigureAwait(false)) {
                stores.Add(macStore);
            }
        }
        else if (OperatingSystem.IsLinux()) {
            var linuxStore = _serviceProvider.GetRequiredService<LinuxCredentialStore>();
            if (await linuxStore.IsAvailableAsync(cancellationToken).ConfigureAwait(false)) {
                stores.Add(linuxStore);
            }
        }

        // Always add fallback store as last resort
        var fallbackStore = _serviceProvider.GetRequiredService<FallbackCredentialStore>();
        if (await fallbackStore.IsAvailableAsync(cancellationToken).ConfigureAwait(false)) {
            stores.Add(fallbackStore);
        }

        return stores;
    }

    /// <inheritdoc />
    public async Task<CredentialStorePlatformInfo> GetPlatformInfoAsync(CancellationToken cancellationToken = default) {
        var platform = GetPlatformName();
        var availableStores = await GetAvailableStoresAsync(cancellationToken).ConfigureAwait(false);

        var storeInfos = availableStores.Select((store, index) => new CredentialStoreInfo {
            Name = store.StorageMechanism,
            IsSecure = store.IsSecure,
            IsAvailable = true,
            Description = GetStoreDescription(store.StorageMechanism),
            PreferenceOrder = index,
        }).ToList();

        return new CredentialStorePlatformInfo {
            Platform = platform,
            PreferredStore = storeInfos.FirstOrDefault()?.Name ?? "None",
            AvailableStores = storeInfos,
            HasSecureStorage = storeInfos.Any(s => s.IsSecure),
            Notes = GetPlatformNotes(platform),
        };
    }

    private static string GetPlatformName() {
        if (OperatingSystem.IsWindows())
            return "Windows";
        if (OperatingSystem.IsMacOS())
            return "macOS";
        if (OperatingSystem.IsLinux())
            return "Linux";
        return OperatingSystem.IsFreeBSD() ? "FreeBSD" : Environment.OSVersion.Platform.ToString();
    }

    private static string GetStoreDescription(string storeMechanism) => storeMechanism switch {
        "Windows Credential Manager" => "Uses Windows Credential Manager for secure storage",
        "macOS Keychain" => "Uses macOS Keychain Services for secure storage",
        "libsecret" => "Uses libsecret for secure storage on Linux",
        "Encrypted File" => "Uses AES-256 encrypted local file storage",
        _ => "Platform-specific secure storage",
    };

    private static string? GetPlatformNotes(string platform) => platform switch {
        "Windows" => "Credentials are stored in Windows Credential Manager and accessible only to the current user",
        "macOS" => "Credentials are stored in the user's keychain and protected by macOS security",
        "Linux" => "Attempts to use libsecret if available, otherwise uses encrypted file storage",
        _ => null,
    };
}