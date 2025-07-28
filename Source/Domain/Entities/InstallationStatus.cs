namespace MCPHub.Domain.Entities;

/// <summary>
/// Represents the status of a package installation
/// </summary>
public enum InstallationStatus
{
    /// <summary>
    /// Installation has been initiated but not yet completed
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Installation completed successfully
    /// </summary>
    Completed = 1,

    /// <summary>
    /// Installation failed due to an error
    /// </summary>
    Failed = 2,

    /// <summary>
    /// Package was successfully uninstalled
    /// </summary>
    Uninstalled = 3,

    /// <summary>
    /// Installation was cancelled by the user
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// Installation timed out
    /// </summary>
    TimedOut = 5
}