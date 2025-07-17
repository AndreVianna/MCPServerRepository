using Common.Messaging;

using Domain.Commands;
using Domain.Events;

using Microsoft.AspNetCore.Mvc;

namespace PublicApi.Controllers;

/// <summary>
/// Controller for server management operations
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
    public async Task<IActionResult> RegisterServer([FromBody] RegisterServerRequest request, CancellationToken cancellationToken) {
        try {
            var serverId = Guid.NewGuid().ToString();
            var correlationId = Guid.NewGuid().ToString();
            var initiatedBy = User.Identity?.Name ?? "anonymous";

            _logger.LogInformation("Registering server {ServerName} for publisher {PublisherId}",
                request.Name, request.PublisherId);

            // Publish server registered event
            var serverRegisteredEvent = new ServerRegisteredEvent(
                serverId,
                request.Name,
                request.Description,
                request.PublisherId,
                correlationId,
                initiatedBy);

            await _messagePublisher.PublishAsync(serverRegisteredEvent, cancellationToken);

            // Optionally trigger immediate security scan
            if (request.RequireImmediateScan) {
                var scanCommand = new ScanServerCommand(
                    serverId,
                    serverId, // Using serverId as version for this example
                    "StaticAnalysis",
                    correlationId,
                    initiatedBy);

                await _messagePublisher.PublishAsync(scanCommand, "commands", "security.scan", cancellationToken);
            }

            var response = new RegisterServerResponse {
                ServerId = serverId,
                Status = "Registered",
                Message = "Server registered successfully. Security scan and indexing will be performed automatically."
            };

            return Ok(response);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to register server {ServerName}", request.Name);
            return StatusCode(500, new { Error = "Failed to register server", Details = ex.Message });
        }
    }

    /// <summary>
    /// Triggers a manual security scan for a server
    /// </summary>
    /// <param name="serverId">The server ID</param>
    /// <param name="request">The scan request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The scan result</returns>
    [HttpPost("{serverId}/scan")]
    public async Task<IActionResult> ScanServer(string serverId, [FromBody] ScanServerRequest request, CancellationToken cancellationToken) {
        try {
            var correlationId = Guid.NewGuid().ToString();
            var initiatedBy = User.Identity?.Name ?? "anonymous";

            _logger.LogInformation("Triggering security scan for server {ServerId}, scan type {ScanType}",
                serverId, request.ScanType);

            var scanCommand = new ScanServerCommand(
                serverId,
                request.ServerVersionId,
                request.ScanType,
                correlationId,
                initiatedBy);

            await _messagePublisher.PublishAsync(scanCommand, "commands", "security.scan", cancellationToken);

            var response = new ScanServerResponse {
                ServerId = serverId,
                ScanType = request.ScanType,
                Status = "Initiated",
                Message = "Security scan has been initiated. Results will be available shortly."
            };

            return Ok(response);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to initiate security scan for server {ServerId}", serverId);
            return StatusCode(500, new { Error = "Failed to initiate security scan", Details = ex.Message });
        }
    }

    /// <summary>
    /// Triggers manual indexing for a server
    /// </summary>
    /// <param name="serverId">The server ID</param>
    /// <param name="request">The index request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The index result</returns>
    [HttpPost("{serverId}/index")]
    public async Task<IActionResult> IndexServer(string serverId, [FromBody] IndexServerRequest request, CancellationToken cancellationToken) {
        try {
            var correlationId = Guid.NewGuid().ToString();
            var initiatedBy = User.Identity?.Name ?? "anonymous";

            _logger.LogInformation("Triggering search indexing for server {ServerId}, index type {IndexType}",
                serverId, request.IndexType);

            var indexCommand = new IndexServerCommand(
                serverId,
                request.ServerVersionId,
                request.IndexType,
                correlationId,
                initiatedBy);

            await _messagePublisher.PublishAsync(indexCommand, "commands", "search.index", cancellationToken);

            var response = new IndexServerResponse {
                ServerId = serverId,
                IndexType = request.IndexType,
                Status = "Initiated",
                Message = "Search indexing has been initiated. The server will be available in search results shortly."
            };

            return Ok(response);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to initiate search indexing for server {ServerId}", serverId);
            return StatusCode(500, new { Error = "Failed to initiate search indexing", Details = ex.Message });
        }
    }
}

/// <summary>
/// Request to register a new server
/// </summary>
public class RegisterServerRequest {
    /// <summary>
    /// The name of the server
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the server
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the publisher
    /// </summary>
    public string PublisherId { get; set; } = string.Empty;

    /// <summary>
    /// Whether to require immediate security scan
    /// </summary>
    public bool RequireImmediateScan { get; set; } = false;
}

/// <summary>
/// Response for server registration
/// </summary>
public class RegisterServerResponse {
    /// <summary>
    /// The ID of the registered server
    /// </summary>
    public string ServerId { get; set; } = string.Empty;

    /// <summary>
    /// The status of the registration
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Additional message
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Request to scan a server
/// </summary>
public class ScanServerRequest {
    /// <summary>
    /// The ID of the server version to scan
    /// </summary>
    public string ServerVersionId { get; set; } = string.Empty;

    /// <summary>
    /// The type of scan to perform
    /// </summary>
    public string ScanType { get; set; } = "StaticAnalysis";
}

/// <summary>
/// Response for server scan
/// </summary>
public class ScanServerResponse {
    /// <summary>
    /// The ID of the server
    /// </summary>
    public string ServerId { get; set; } = string.Empty;

    /// <summary>
    /// The type of scan
    /// </summary>
    public string ScanType { get; set; } = string.Empty;

    /// <summary>
    /// The status of the scan
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Additional message
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Request to index a server
/// </summary>
public class IndexServerRequest {
    /// <summary>
    /// The ID of the server version to index
    /// </summary>
    public string ServerVersionId { get; set; } = string.Empty;

    /// <summary>
    /// The type of indexing to perform
    /// </summary>
    public string IndexType { get; set; } = "Full";
}

/// <summary>
/// Response for server indexing
/// </summary>
public class IndexServerResponse {
    /// <summary>
    /// The ID of the server
    /// </summary>
    public string ServerId { get; set; } = string.Empty;

    /// <summary>
    /// The type of indexing
    /// </summary>
    public string IndexType { get; set; } = string.Empty;

    /// <summary>
    /// The status of the indexing
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Additional message
    /// </summary>
    public string Message { get; set; } = string.Empty;
}