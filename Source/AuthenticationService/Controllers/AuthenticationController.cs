using MCPHub.AuthenticationService.Services;
using MCPHub.Domain.Contracts.Requests;

using Microsoft.AspNetCore.Mvc;

namespace MCPHub.AuthenticationService.Controllers;

/// <summary>
/// Authentication controller for user login, registration, and token management
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase {
    private readonly IAuthenticationService _authenticationService = authenticationService;

    /// <summary>
    /// Authenticates a user and returns access tokens
    /// </summary>
    /// <param name="request">Login request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication result with tokens</returns>
    [HttpPost("login")]
    public Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Login endpoint will be implemented when first consumer requires it");

    /// <summary>
    /// Registers a new user account
    /// </summary>
    /// <param name="request">Registration request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Registration result</returns>
    [HttpPost("register")]
    public Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Registration endpoint will be implemented when first consumer requires it");

    /// <summary>
    /// Refreshes an access token using a refresh token
    /// </summary>
    /// <param name="request">Token refresh request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New access token</returns>
    [HttpPost("refresh")]
    public Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Token refresh endpoint will be implemented when first consumer requires it");

    /// <summary>
    /// Logs out a user and invalidates tokens
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Logout result</returns>
    [HttpPost("logout")]
    public Task<IActionResult> LogoutAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Logout endpoint will be implemented when first consumer requires it");
}