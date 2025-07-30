namespace MCPHub.AuthenticationService.Controllers;

/// <summary>
/// User profile management controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserProfileController(IUserProfileService userProfileService) : ControllerBase {
    private readonly IUserProfileService _userProfileService = userProfileService;

    /// <summary>
    /// Gets the current user's profile information
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User profile information</returns>
    [HttpGet("me")]
    public Task<IActionResult> GetMyProfileAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Get profile endpoint will be implemented when first consumer requires it");

    /// <summary>
    /// Gets a user's profile by ID
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User profile information</returns>
    [HttpGet("{userId:guid}")]
    public Task<IActionResult> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Get user profile endpoint will be implemented when first consumer requires it");

    /// <summary>
    /// Updates the current user's profile
    /// </summary>
    /// <param name="request">Profile update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Profile update result</returns>
    [HttpPut("me")]
    public Task<IActionResult> UpdateMyProfileAsync([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Update profile endpoint will be implemented when first consumer requires it");

    /// <summary>
    /// Changes the current user's password
    /// </summary>
    /// <param name="request">Password change request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Password change result</returns>
    [HttpPost("change-password")]
    public Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Change password endpoint will be implemented when first consumer requires it");

    /// <summary>
    /// Verifies user email address
    /// </summary>
    /// <param name="request">Email verification request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Email verification result</returns>
    [HttpPost("verify-email")]
    public Task<IActionResult> VerifyEmailAsync([FromBody] VerifyEmailRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Email verification endpoint will be implemented when first consumer requires it");
}