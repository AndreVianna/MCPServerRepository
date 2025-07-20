namespace MCPHub.Storage;

/// <summary>
/// Storage security settings
/// </summary>
public class StorageSecuritySettings {
    public bool EnableEncryptionAtRest { get; set; } = true;
    public bool EnableEncryptionInTransit { get; set; } = true;
    public string? EncryptionKey { get; set; }
    public string? KeyVaultUrl { get; set; }
    public List<string> AllowedIpAddresses { get; set; } = [];
    public List<string> BlockedIpAddresses { get; set; } = [];
    public bool EnableAccessLogging { get; set; } = true;
    public bool EnableVirusScanning { get; set; } = true;
    public int MaxDownloadAttemptsPerHour { get; set; } = 100;
}