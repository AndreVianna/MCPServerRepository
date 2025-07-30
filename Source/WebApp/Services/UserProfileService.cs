using MCPHub.Domain.Contracts.Requests;
using MCPHub.Domain.Contracts.Responses;

namespace MCPHub.WebApp.Services;

/// <summary>
/// Service for user profile management operations
/// </summary>
public class UserProfileService(IApiClientService apiClient, ILogger<UserProfileService> logger) : IUserProfileService {
    private readonly IApiClientService _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
    private readonly ILogger<UserProfileService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public Task<UserProfileResult> GetProfileAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException("User profile retrieval will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<UserProfileUpdateResult> UpdateProfileAsync(UpdateUserProfileRequest request, CancellationToken cancellationToken = default) => throw new NotImplementedException("Profile update will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<AvatarUploadResult> UpdateAvatarAsync(UpdateAvatarRequest request, CancellationToken cancellationToken = default) => throw new NotImplementedException("Avatar upload will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<ActivityLogResult> GetActivityLogAsync(int pageSize = 20, int pageNumber = 1, CancellationToken cancellationToken = default) => throw new NotImplementedException("Activity log retrieval will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<UserStatistics?> GetUserStatisticsAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException("User statistics retrieval will be implemented when first consumer requires it");
}