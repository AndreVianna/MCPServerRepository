using MCPHub.Domain.Contracts.Requests;
using MCPHub.Domain.Contracts.Responses;
using MCPHub.Domain.Entities;
using MCPHub.PublicApi.Services;

namespace MCPHub.PublicApi.Controllers;

/// <summary>
/// Controller for authentication operations
/// </summary>
/// <param name="userManager">User manager for Identity operations</param>
/// <param name="signInManager">Sign in manager for authentication</param>
/// <param name="jwtService">JWT service for token operations</param>
/// <param name="logger">Logger for request tracking</param>
[ApiController]
[Route("api/[controller]")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtService jwtService,
    ILogger<AuthController> logger) : ControllerBase {
    private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
    private readonly IJwtService _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
    private readonly ILogger<AuthController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Authenticates a user and returns JWT tokens
    /// </summary>
    /// <param name="request">Login request with email and password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication result with JWT tokens</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken) {
        try {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            // Find user by email
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) {
                _logger.LogWarning("Login failed - user not found for email: {Email}", request.Email);
                return BadRequest(new AuthenticationResult {
                    IsSuccess = false,
                    ErrorMessage = "Invalid email or password"
                });
            }

            // Check password
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (!result.Succeeded) {
                _logger.LogWarning("Login failed for user {UserId}: {Reason}", user.Id,
                    result.IsLockedOut ? "Account locked" : "Invalid password");

                var errorMessage = result.IsLockedOut ? "Account is locked out" : "Invalid email or password";
                return BadRequest(new AuthenticationResult {
                    IsSuccess = false,
                    ErrorMessage = errorMessage
                });
            }

            // Generate tokens
            var accessToken = await _jwtService.GenerateAccessTokenAsync(user, cancellationToken);
            var refreshToken = await _jwtService.GenerateRefreshTokenAsync(user, cancellationToken);
            var expiresAt = DateTime.UtcNow.AddMinutes(15); // TODO: Get from configuration

            _logger.LogInformation("Login successful for user {UserId}", user.Id);

            return Ok(new AuthenticationResult {
                IsSuccess = true,
                User = user,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt
            });
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error occurred during login for email: {Email}", request.Email);
            return StatusCode(500, new AuthenticationResult {
                IsSuccess = false,
                ErrorMessage = "An error occurred while processing your request"
            });
        }
    }

    /// <summary>
    /// Refreshes an expired access token using a refresh token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New JWT tokens</returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken) {
        try {
            _logger.LogInformation("Token refresh attempt");

            // Validate refresh token
            var userId = await _jwtService.ValidateRefreshTokenAsync(request.RefreshToken, cancellationToken);
            if (userId == null) {
                _logger.LogWarning("Token refresh failed - invalid refresh token");
                return BadRequest(new TokenResult {
                    IsSuccess = false,
                    ErrorMessage = "Invalid refresh token"
                });
            }

            // Get user
            var user = await _userManager.FindByIdAsync(userId.ToString()!);
            if (user == null) {
                _logger.LogWarning("Token refresh failed - user not found for ID: {UserId}", userId);
                return BadRequest(new TokenResult {
                    IsSuccess = false,
                    ErrorMessage = "User not found"
                });
            }

            // Generate new tokens
            var accessToken = await _jwtService.GenerateAccessTokenAsync(user, cancellationToken);
            var refreshToken = await _jwtService.GenerateRefreshTokenAsync(user, cancellationToken);
            var expiresAt = DateTime.UtcNow.AddMinutes(15); // TODO: Get from configuration

            // Revoke old refresh token
            await _jwtService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);

            _logger.LogInformation("Token refresh successful for user {UserId}", user.Id);

            return Ok(new TokenResult {
                IsSuccess = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt
            });
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error occurred during token refresh");
            return StatusCode(500, new TokenResult {
                IsSuccess = false,
                ErrorMessage = "An error occurred while processing your request"
            });
        }
    }

    /// <summary>
    /// Logs out a user and invalidates their refresh token
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Logout result</returns>
    [HttpPost("logout")]
    [Authorize]
    public Task<IActionResult> Logout(CancellationToken cancellationToken) {
        try {
            var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId)) {
                _logger.LogWarning("Logout failed - invalid user ID in token");
                return Task.FromResult<IActionResult>(BadRequest(new LogoutResult {
                    IsSuccess = false,
                    ErrorMessage = "Invalid user context"
                }));
            }

            _logger.LogInformation("Logout for user {UserId}", userId);

            // TODO: Implement refresh token revocation logic
            // For now, we'll just return success as the access token will expire naturally

            return Task.FromResult<IActionResult>(Ok(new LogoutResult {
                IsSuccess = true
            }));
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error occurred during logout");
            return Task.FromResult<IActionResult>(StatusCode(500, new LogoutResult {
                IsSuccess = false,
                ErrorMessage = "An error occurred while processing your request"
            }));
        }
    }

    /// <summary>
    /// Gets the current user's profile information
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User profile information</returns>
    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken) {
        try {
            var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId)) {
                _logger.LogWarning("Get profile failed - invalid user ID in token");
                return BadRequest(new UserProfileResult {
                    IsSuccess = false,
                    ErrorMessage = "Invalid user context"
                });
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) {
                _logger.LogWarning("Get profile failed - user not found for ID: {UserId}", userId);
                return NotFound(new UserProfileResult {
                    IsSuccess = false,
                    ErrorMessage = "User not found"
                });
            }

            _logger.LogInformation("Profile retrieved for user {UserId}", userId);

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
                LastLoginAt = null
            };

            return Ok(new UserProfileResult {
                IsSuccess = true,
                Profile = profile
            });
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error occurred while getting user profile");
            return StatusCode(500, new UserProfileResult {
                IsSuccess = false,
                ErrorMessage = "An error occurred while processing your request"
            });
        }
    }
}