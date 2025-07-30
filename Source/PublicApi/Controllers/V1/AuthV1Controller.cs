using Asp.Versioning;

using MCPHub.Domain.Contracts.Requests;
using MCPHub.Domain.Contracts.Responses;
using MCPHub.Domain.Entities;
using MCPHub.PublicApi.Services;

namespace MCPHub.PublicApi.Controllers.V1;

/// <summary>
/// Controller for authentication operations - API Version 1.0
/// </summary>
/// <param name="userManager">User manager for Identity operations</param>
/// <param name="signInManager">Sign in manager for authentication</param>
/// <param name="jwtService">JWT service for token operations</param>
/// <param name="logger">Logger for request tracking</param>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthV1Controller(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtService jwtService,
    ILogger<AuthV1Controller> logger) : BaseApiV1Controller(logger) {
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
    private readonly IJwtService _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));

    /// <summary>
    /// Authenticates a user and returns JWT tokens
    /// </summary>
    /// <param name="request">Login request with email and password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication result with JWT tokens</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthenticationResult), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken) {
        try {
            Logger.LogInformation("Login attempt for email: {Email}", request.Email);

            // Find user by email
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) {
                Logger.LogWarning("Login failed - user not found for email: {Email}", request.Email);
                return CreateErrorResponse("Invalid email or password", 400);
            }

            // Check password
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (!result.Succeeded) {
                Logger.LogWarning("Login failed for user {UserId}: {Reason}", user.Id,
                    result.IsLockedOut ? "Account locked" : "Invalid password");

                var errorMessage = result.IsLockedOut ? "Account is locked out" : "Invalid email or password";
                return CreateErrorResponse(errorMessage, 400);
            }

            // Generate tokens
            var accessToken = await _jwtService.GenerateAccessTokenAsync(user, cancellationToken);
            var refreshToken = await _jwtService.GenerateRefreshTokenAsync(user, cancellationToken);
            var expiresAt = DateTime.UtcNow.AddMinutes(15); // TODO: Get from configuration

            Logger.LogInformation("Login successful for user {UserId}", user.Id);

            var authResult = new AuthenticationResult {
                IsSuccess = true,
                User = user,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
            };

            return CreateSuccessResponse(authResult, "Login successful");
        }
        catch (Exception ex) {
            Logger.LogError(ex, "Error occurred during login for email: {Email}", request.Email);
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }

    /// <summary>
    /// Refreshes an expired access token using a refresh token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New JWT tokens</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(TokenResult), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken) {
        try {
            Logger.LogInformation("Token refresh attempt");

            // Validate refresh token
            var userId = await _jwtService.ValidateRefreshTokenAsync(request.RefreshToken, cancellationToken);
            if (userId == null) {
                Logger.LogWarning("Token refresh failed - invalid refresh token");
                return CreateErrorResponse("Invalid refresh token", 400);
            }

            // Get user
            var user = await _userManager.FindByIdAsync(userId.ToString()!);
            if (user == null) {
                Logger.LogWarning("Token refresh failed - user not found for ID: {UserId}", userId);
                return CreateErrorResponse("User not found", 400);
            }

            // Generate new tokens
            var accessToken = await _jwtService.GenerateAccessTokenAsync(user, cancellationToken);
            var refreshToken = await _jwtService.GenerateRefreshTokenAsync(user, cancellationToken);
            var expiresAt = DateTime.UtcNow.AddMinutes(15); // TODO: Get from configuration

            // Revoke old refresh token
            await _jwtService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);

            Logger.LogInformation("Token refresh successful for user {UserId}", user.Id);

            var tokenResult = new TokenResult {
                IsSuccess = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
            };

            return CreateSuccessResponse(tokenResult, "Token refresh successful");
        }
        catch (Exception ex) {
            Logger.LogError(ex, "Error occurred during token refresh");
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }

    /// <summary>
    /// Logs out a user and invalidates their refresh token
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Logout result</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(LogoutResult), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 500)]
    public Task<IActionResult> Logout(CancellationToken cancellationToken) {
        try {
            var userId = GetCurrentUserId();
            if (userId == null) {
                Logger.LogWarning("Logout failed - invalid user ID in token");
                return Task.FromResult(CreateErrorResponse("Invalid user context", 400));
            }

            Logger.LogInformation("Logout for user {UserId}", userId);

            // TODO: Implement refresh token revocation logic
            // For now, we'll just return success as the access token will expire naturally

            var logoutResult = new LogoutResult { IsSuccess = true };
            return Task.FromResult(CreateSuccessResponse(logoutResult, "Logout successful"));
        }
        catch (Exception ex) {
            Logger.LogError(ex, "Error occurred during logout");
            return Task.FromResult(CreateErrorResponse("An error occurred while processing your request", 500));
        }
    }

    /// <summary>
    /// Gets the current user's profile information
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User profile information</returns>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileResult), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken) {
        try {
            var userId = GetCurrentUserId();
            if (userId == null) {
                Logger.LogWarning("Get profile failed - invalid user ID in token");
                return CreateErrorResponse("Invalid user context", 400);
            }

            var user = await _userManager.FindByIdAsync(userId.ToString()!);
            if (user == null) {
                Logger.LogWarning("Get profile failed - user not found for ID: {UserId}", userId);
                return CreateErrorResponse("User not found", 404);
            }

            Logger.LogInformation("Profile retrieved for user {UserId}", userId);

            // Map ApplicationUser to UserProfile
            var profile = new UserProfile {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                DisplayName = user.DisplayName,
                Bio = user.Bio,
                Website = user.Website,
                GitHubUrl = user.GitHubUsername != null ? $"https://github.com/{user.GitHubUsername}" : null,
                TwitterUrl = user.TwitterHandle != null ? $"https://twitter.com/{user.TwitterHandle}" : null,
                AvatarUrl = user.AvatarUrl,
                AccountType = user.IsPublisher ? "Publisher" : "User",
                IsEmailVerified = user.IsEmailVerified,
                IsPublicProfile = true,
                ShowEmail = false,
                ReceiveUpdates = false,
                ReceiveMarketingEmails = false,
                IsTwoFactorEnabled = user.TwoFactorEnabled,
                CreatedAt = DateTime.UtcNow, // TODO: Get from audit trail
                LastLoginAt = null,
            };

            var profileResult = new UserProfileResult {
                IsSuccess = true,
                Profile = profile,
            };

            return CreateSuccessResponse(profileResult, "Profile retrieved successfully");
        }
        catch (Exception ex) {
            Logger.LogError(ex, "Error occurred while getting user profile");
            return CreateErrorResponse("An error occurred while processing your request", 500);
        }
    }
}