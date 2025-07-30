using MCPHub.Domain.Entities;
using MCPHub.Domain.Messaging;

namespace MCPHub.Domain.Events;

/// <summary>
/// Domain event triggered when a package installation is completed successfully
/// </summary>
public record PackageInstalledEvent : BaseMessage {
    /// <summary>
    /// Gets or sets the installation tracking identifier
    /// </summary>
    public required Guid InstallationId { get; init; }

    /// <summary>
    /// Gets or sets the package identifier that was installed
    /// </summary>
    public required Guid PackageId { get; init; }

    /// <summary>
    /// Gets or sets the package name
    /// </summary>
    public required string PackageName { get; init; }

    /// <summary>
    /// Gets or sets the version of the package that was installed
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    /// Gets or sets the user ID who installed the package
    /// </summary>
    public new required Guid UserId { get; init; }

    /// <summary>
    /// Gets or sets the installation path where the package was installed
    /// </summary>
    public required string InstallationPath { get; init; }

    /// <summary>
    /// Gets or sets the timestamp when the installation was completed
    /// </summary>
    public required DateTimeOffset InstalledAt { get; init; }

    /// <summary>
    /// Gets or sets the client version used for installation
    /// </summary>
    public required string ClientVersion { get; init; }

    /// <summary>
    /// Gets or sets the installation options used
    /// </summary>
    public Dictionary<string, object>? InstallationOptions { get; init; }

    /// <summary>
    /// Creates a new PackageInstalledEvent
    /// </summary>
    /// <param name="installationId">Installation tracking identifier</param>
    /// <param name="packageId">Package identifier</param>
    /// <param name="packageName">Package name</param>
    /// <param name="version">Package version</param>
    /// <param name="userId">User identifier</param>
    /// <param name="installationPath">Installation path</param>
    /// <param name="installedAt">Installation completion timestamp</param>
    /// <param name="clientVersion">Client version</param>
    /// <param name="installationOptions">Installation options (optional)</param>
    /// <returns>New PackageInstalledEvent instance</returns>
    public static PackageInstalledEvent Create(
        Guid installationId,
        Guid packageId,
        string packageName,
        string version,
        Guid userId,
        string installationPath,
        DateTimeOffset installedAt,
        string clientVersion,
        Dictionary<string, object>? installationOptions = null) => new() {
            InstallationId = installationId,
            PackageId = packageId,
            PackageName = packageName,
            Version = version,
            UserId = userId,
            InstallationPath = installationPath,
            InstalledAt = installedAt,
            ClientVersion = clientVersion,
            InstallationOptions = installationOptions
        };
}