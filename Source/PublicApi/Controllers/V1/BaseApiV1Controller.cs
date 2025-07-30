using System.Diagnostics;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

namespace MCPHub.PublicApi.Controllers.V1;

/// <summary>
/// Base controller for API version 1.0
/// </summary>
/// <remarks>
/// Initializes a new instance of the base API v1 controller
/// </remarks>
/// <param name="logger">Logger instance</param>
[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
public abstract class BaseApiV1Controller(ILogger logger) : ControllerBase {
    /// <summary>
    /// Logger instance for derived controllers
    /// </summary>
    protected ILogger Logger { get; } = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Creates a standardized error response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <returns>Error response</returns>
    protected IActionResult CreateErrorResponse(string message, int statusCode = 400) {
        var errorResponse = new {
            Error = new {
                Message = message,
                Timestamp = DateTimeOffset.UtcNow,
                TraceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Version = "1.0"
            }
        };

        return StatusCode(statusCode, errorResponse);
    }

    /// <summary>
    /// Creates a standardized success response
    /// </summary>
    /// <typeparam name="T">Response data type</typeparam>
    /// <param name="data">Response data</param>
    /// <param name="message">Success message</param>
    /// <returns>Success response</returns>
    protected IActionResult CreateSuccessResponse<T>(T data, string? message = null) {
        var response = new {
            Data = data,
            Message = message,
            Timestamp = DateTimeOffset.UtcNow,
            Version = "1.0"
        };

        return Ok(response);
    }

    /// <summary>
    /// Gets the current user ID from claims
    /// </summary>
    /// <returns>User ID if authenticated, null otherwise</returns>
    protected Guid? GetCurrentUserId() {
        if (User.Identity?.IsAuthenticated != true)
            return null;

        var userIdClaim = User.FindFirst("sub")?.Value ??
                         User.FindFirst("id")?.Value ??
                         User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    /// <summary>
    /// Gets the current user's roles
    /// </summary>
    /// <returns>List of user roles</returns>
    protected IEnumerable<string> GetCurrentUserRoles() {
        if (User.Identity?.IsAuthenticated != true)
            return Enumerable.Empty<string>();

        return User.Claims
            .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
            .Select(c => c.Value);
    }

    /// <summary>
    /// Checks if the current user has a specific role
    /// </summary>
    /// <param name="role">Role to check</param>
    /// <returns>True if user has the role</returns>
    protected bool HasRole(string role) => GetCurrentUserRoles().Contains(role, StringComparer.OrdinalIgnoreCase);
}