using MCPHub.Common.Messaging;
using MCPHub.Domain.Contracts.Requests;

namespace MCPHub.PublicApi.Controllers;

/// <summary>
/// Controller for server management operations - following contracts-first approach
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ServersController(IMessagePublisher messagePublisher, ILogger<ServersController> logger) : ControllerBase {
    private readonly IMessagePublisher _messagePublisher = messagePublisher;
    private readonly ILogger<ServersController> _logger = logger;

    /// <summary>
    /// Registers a new server
    /// </summary>
    /// <param name="request">The server registration request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The registration result</returns>
    [HttpPost]
    public Task<IActionResult> RegisterServer([FromBody] RegisterServerRequest request, CancellationToken cancellationToken)
        => throw new NotImplementedException("Server registration will be implemented when persistence layer is available");

    /// <summary>
    /// Triggers a manual security scan for a server
    /// </summary>
    /// <param name="serverId">The server ID</param>
    /// <param name="request">The scan request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The scan result</returns>
    [HttpPost("{serverId}/scan")]
    public Task<IActionResult> ScanServer(string serverId, [FromBody] ScanServerRequest request, CancellationToken cancellationToken)
        => throw new NotImplementedException("Manual security scanning will be implemented when security service integration is available");

    /// <summary>
    /// Triggers manual indexing for a server
    /// </summary>
    /// <param name="serverId">The server ID</param>
    /// <param name="request">The index request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The index result</returns>
    [HttpPost("{serverId}/index")]
    public Task<IActionResult> IndexServer(string serverId, [FromBody] IndexServerRequest request, CancellationToken cancellationToken)
        => throw new NotImplementedException("Manual indexing will be implemented when search service integration is available");
}