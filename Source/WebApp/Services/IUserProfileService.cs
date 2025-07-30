using MCPHub.Domain.Contracts.Requests;
using MCPHub.Domain.Contracts.Responses;

namespace MCPHub.WebApp.Services;

/// <summary>
/// Interface for user profile management operations
/// </summary>
public interface IUserProfileService {
    /// <summary>
    /// Gets the current user's profile
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User profile result</returns>
    Task<UserProfileResult> GetProfileAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the current user's profile information
    /// </summary>
    /// <param name="request">Profile update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Profile update result</returns>
    Task<UserProfileUpdateResult> UpdateProfileAsync(UpdateUserProfileRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads and updates user avatar
    /// </summary>
    /// <param name="request">Avatar update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Avatar upload result</returns>
    Task<AvatarUploadResult> UpdateAvatarAsync(UpdateAvatarRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user activity log
    /// </summary>
    /// <param name="pageSize">Number of entries per page</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Activity log result</returns>
    Task<ActivityLogResult> GetActivityLogAsync(int pageSize = 20, int pageNumber = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user statistics and metrics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User statistics</returns>
    Task<UserStatistics?> GetUserStatisticsAsync(CancellationToken cancellationToken = default);
}