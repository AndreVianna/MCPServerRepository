using System.Diagnostics.CodeAnalysis;
using MCPHub.Domain.Common;

namespace MCPHub.Domain.Entities;

/// <summary>
/// Represents a package installation record for tracking and management
/// </summary>
public class PackageInstallation : BaseEntity
{
    /// <summary>
    /// Gets or sets the package identifier that was installed
    /// </summary>
    public Guid PackageId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the package
    /// </summary>
    public Package Package { get; set; } = null!;

    /// <summary>
    /// Gets or sets the version of the package that was installed
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user ID who installed the package (required for installations)
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the installation path where the package was installed
    /// </summary>
    public string InstallationPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the installation was initiated
    /// </summary>
    public DateTimeOffset InstalledAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the timestamp when the package was uninstalled (null if still installed)
    /// </summary>
    public DateTimeOffset? UninstalledAt { get; set; }

    /// <summary>
    /// Gets or sets the current status of the installation
    /// </summary>
    public InstallationStatus Status { get; set; } = InstallationStatus.Pending;

    /// <summary>
    /// Gets or sets any error message if the installation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets installation options and configuration
    /// </summary>
    public Dictionary<string, object> InstallationOptions { get; set; } = new();

    /// <summary>
    /// Gets or sets the client version used for installation
    /// </summary>
    public string ClientVersion { get; set; } = string.Empty;

    private PackageInstallation() { } // For EF Core

    [SetsRequiredMembers]
    public PackageInstallation(
        Guid packageId,
        string version,
        Guid userId,
        string installationPath,
        string clientVersion,
        Dictionary<string, object>? installationOptions = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        ArgumentException.ThrowIfNullOrWhiteSpace(installationPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientVersion);

        PackageId = packageId;
        Version = version;
        UserId = userId;
        InstallationPath = installationPath;
        ClientVersion = clientVersion;
        Status = InstallationStatus.Pending;
        InstalledAt = DateTimeOffset.UtcNow;
        InstallationOptions = installationOptions ?? new Dictionary<string, object>();

        AuditTrail.Add(new AuditEntry
        {
            Action = "Installation Initiated",
            UserId = userId,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Updates the installation status
    /// </summary>
    /// <param name="status">New installation status</param>
    /// <param name="errorMessage">Error message if status is Failed</param>
    /// <param name="userId">User performing the update</param>
    public void UpdateStatus(InstallationStatus status, string? errorMessage = null, Guid? userId = null)
    {
        var previousStatus = Status;
        Status = status;
        ErrorMessage = errorMessage;

        if (status == InstallationStatus.Uninstalled)
        {
            UninstalledAt = DateTimeOffset.UtcNow;
        }

        AuditTrail.Add(new AuditEntry
        {
            Action = $"Status Updated from {previousStatus} to {status}",
            UserId = userId ?? UserId,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Marks the installation as completed successfully
    /// </summary>
    /// <param name="userId">User completing the installation</param>
    public void MarkCompleted(Guid? userId = null)
    {
        UpdateStatus(InstallationStatus.Completed, null, userId);
    }

    /// <summary>
    /// Marks the installation as failed with an error message
    /// </summary>
    /// <param name="errorMessage">Description of the failure</param>
    /// <param name="userId">User reporting the failure</param>
    public void MarkFailed(string errorMessage, Guid? userId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);
        UpdateStatus(InstallationStatus.Failed, errorMessage, userId);
    }

    /// <summary>
    /// Marks the package as uninstalled
    /// </summary>
    /// <param name="userId">User performing the uninstallation</param>
    public void MarkUninstalled(Guid? userId = null)
    {
        UpdateStatus(InstallationStatus.Uninstalled, null, userId);
    }

    /// <summary>
    /// Updates the installation options
    /// </summary>
    /// <param name="options">New options to add or update</param>
    /// <param name="userId">User updating the options</param>
    public void UpdateInstallationOptions(Dictionary<string, object> options, Guid? userId = null)
    {
        ArgumentNullException.ThrowIfNull(options);

        foreach (var kvp in options)
        {
            InstallationOptions[kvp.Key] = kvp.Value;
        }

        AuditTrail.Add(new AuditEntry
        {
            Action = "Installation Options Updated",
            UserId = userId ?? UserId,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Gets whether the installation is currently active (completed and not uninstalled)
    /// </summary>
    public bool IsActive => Status == InstallationStatus.Completed && UninstalledAt == null;

    /// <summary>
    /// Gets whether the installation can be uninstalled (is currently completed)
    /// </summary>
    public bool CanBeUninstalled => Status == InstallationStatus.Completed && UninstalledAt == null;
}