using MCPHub.Domain.Messaging;

namespace MCPHub.Domain.Events;

/// <summary>
/// Domain event triggered when a package is uninstalled
/// </summary>
public record PackageUninstalledEvent : BaseMessage
{
    /// <summary>
    /// Gets or sets the installation tracking identifier
    /// </summary>
    public required Guid InstallationId { get; init; }

    /// <summary>
    /// Gets or sets the package identifier that was uninstalled
    /// </summary>
    public required Guid PackageId { get; init; }

    /// <summary>
    /// Gets or sets the package name
    /// </summary>
    public required string PackageName { get; init; }

    /// <summary>
    /// Gets or sets the version of the package that was uninstalled
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    /// Gets or sets the user ID who uninstalled the package
    /// </summary>
    public new required Guid UserId { get; init; }

    /// <summary>
    /// Gets or sets the installation path from which the package was uninstalled
    /// </summary>
    public required string InstallationPath { get; init; }

    /// <summary>
    /// Gets or sets the timestamp when the package was originally installed
    /// </summary>
    public required DateTimeOffset OriginalInstalledAt { get; init; }

    /// <summary>
    /// Gets or sets the timestamp when the package was uninstalled
    /// </summary>
    public required DateTimeOffset UninstalledAt { get; init; }

    /// <summary>
    /// Gets or sets the reason for uninstallation (optional)
    /// </summary>
    public string? UninstallReason { get; init; }

    /// <summary>
    /// Gets or sets the client version used for uninstallation
    /// </summary>
    public string? ClientVersion { get; init; }

    /// <summary>
    /// Creates a new PackageUninstalledEvent
    /// </summary>
    /// <param name="installationId">Installation tracking identifier</param>
    /// <param name="packageId">Package identifier</param>
    /// <param name="packageName">Package name</param>
    /// <param name="version">Package version</param>
    /// <param name="userId">User identifier</param>
    /// <param name="installationPath">Installation path</param>
    /// <param name="originalInstalledAt">Original installation timestamp</param>
    /// <param name="uninstalledAt">Uninstallation timestamp</param>
    /// <param name="uninstallReason">Reason for uninstallation (optional)</param>
    /// <param name="clientVersion">Client version (optional)</param>
    /// <returns>New PackageUninstalledEvent instance</returns>
    public static PackageUninstalledEvent Create(
        Guid installationId,
        Guid packageId,
        string packageName,
        string version,
        Guid userId,
        string installationPath,
        DateTimeOffset originalInstalledAt,
        DateTimeOffset uninstalledAt,
        string? uninstallReason = null,
        string? clientVersion = null)
    {
        return new PackageUninstalledEvent
        {
            InstallationId = installationId,
            PackageId = packageId,
            PackageName = packageName,
            Version = version,
            UserId = userId,
            InstallationPath = installationPath,
            OriginalInstalledAt = originalInstalledAt,
            UninstalledAt = uninstalledAt,
            UninstallReason = uninstallReason,
            ClientVersion = clientVersion
        };
    }
}