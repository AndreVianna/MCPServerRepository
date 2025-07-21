using MCPHub.Domain.Contracts.Requests;
using MCPHub.Domain.Contracts.Responses;
using MCPHub.Domain.Entities;

namespace MCPHub.Domain.Contracts.Services;

/// <summary>
/// Application service interface for server management operations
/// </summary>
public interface IServerService {
    /// <summary>
    /// Registers a new server
    /// </summary>
    /// <param name="request">Server registration request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Server registration response</returns>
    Task<RegisterServerResponse> RegisterServerAsync(RegisterServerRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates a security scan for a server
    /// </summary>
    /// <param name="serverId">Server identifier</param>
    /// <param name="request">Scan request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Scan response</returns>
    Task<ScanServerResponse> ScanServerAsync(string serverId, ScanServerRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates search indexing for a server
    /// </summary>
    /// <param name="serverId">Server identifier</param>
    /// <param name="request">Index request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Index response</returns>
    Task<IndexServerResponse> IndexServerAsync(string serverId, IndexServerRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a server by its identifier
    /// </summary>
    /// <param name="serverId">Server identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Server entity or null if not found</returns>
    Task<Server?> GetServerAsync(string serverId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets servers by publisher
    /// </summary>
    /// <param name="publisherId">Publisher identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of servers for the publisher</returns>
    Task<IEnumerable<Server>> GetServersByPublisherAsync(string publisherId, CancellationToken cancellationToken = default);
}