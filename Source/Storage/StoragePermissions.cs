namespace MCPHub.Storage;

/// <summary>
/// Storage access permissions
/// </summary>
[Flags]
public enum StoragePermissions {
    Read = 1,
    Write = 2,
    Delete = 4,
    ReadWrite = Read | Write,
    Full = Read | Write | Delete
}