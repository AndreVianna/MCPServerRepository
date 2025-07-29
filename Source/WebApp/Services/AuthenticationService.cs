using System.Text;
using System.Text.Json;
using MCPHub.Domain.Contracts.Requests;
using MCPHub.Domain.Contracts.Responses;
using Microsoft.AspNetCore.Components.Authorization;

namespace MCPHub.WebApp.Services;

/// <summary>
/// Authentication service implementation for web application
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly CustomAuthenticationStateProvider _authStateProvider;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public AuthenticationService(
        HttpClient httpClient,
        AuthenticationStateProvider authStateProvider,
        ILogger<AuthenticationService> logger)
    {
        _httpClient = httpClient;
        _authStateProvider = (CustomAuthenticationStateProvider)authStateProvider;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <inheritdoc />
    public async Task<AuthenticationResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/auth/login", content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<AuthenticationResult>(responseContent, _jsonOptions);
                if (result != null && result.IsSuccess && !string.IsNullOrEmpty(result.AccessToken))
                {
                    await _authStateProvider.MarkUserAsAuthenticatedAsync(result.AccessToken, result.RefreshToken ?? string.Empty);
                }
                return result ?? new AuthenticationResult { IsSuccess = false, ErrorMessage = "Invalid response format" };
            }

            var errorResult = JsonSerializer.Deserialize<AuthenticationResult>(responseContent, _jsonOptions);
            return errorResult ?? new AuthenticationResult { IsSuccess = false, ErrorMessage = "Login failed" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return new AuthenticationResult { IsSuccess = false, ErrorMessage = "An error occurred during login" };
        }
    }

    /// <inheritdoc />
    public async Task<RegistrationResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/auth/register", content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            var result = JsonSerializer.Deserialize<RegistrationResult>(responseContent, _jsonOptions);
            return result ?? new RegistrationResult { IsSuccess = false, ErrorMessage = "Registration failed" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return new RegistrationResult { IsSuccess = false, ErrorMessage = "An error occurred during registration" };
        }
    }

    /// <inheritdoc />
    public async Task<TokenResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new RefreshTokenRequest { RefreshToken = refreshToken };
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/auth/refresh", content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<TokenResult>(responseContent, _jsonOptions);
                if (result != null && result.IsSuccess && !string.IsNullOrEmpty(result.AccessToken))
                {
                    await _authStateProvider.MarkUserAsAuthenticatedAsync(result.AccessToken, result.RefreshToken ?? string.Empty);
                }
                return result ?? new TokenResult { IsSuccess = false, ErrorMessage = "Invalid response format" };
            }

            var errorResult = JsonSerializer.Deserialize<TokenResult>(responseContent, _jsonOptions);
            return errorResult ?? new TokenResult { IsSuccess = false, ErrorMessage = "Token refresh failed" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return new TokenResult { IsSuccess = false, ErrorMessage = "An error occurred during token refresh" };
        }
    }

    /// <inheritdoc />
    public async Task<LogoutResult> LogoutAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _authStateProvider.MarkUserAsLoggedOutAsync();
            
            // Optionally call the server logout endpoint
            var response = await _httpClient.PostAsync("/api/auth/logout", null, cancellationToken);
            
            return new LogoutResult { IsSuccess = true, ErrorMessage = null };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            // Even if server call fails, we still clear local state
            await _authStateProvider.MarkUserAsLoggedOutAsync();
            return new LogoutResult { IsSuccess = true, ErrorMessage = null };
        }
    }

    /// <inheritdoc />
    public async Task<UserProfileResult> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await _authStateProvider.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return new UserProfileResult { IsSuccess = false, ErrorMessage = "Not authenticated" };
            }

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("/api/auth/profile", cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<UserProfileResult>(responseContent, _jsonOptions);
                return result ?? new UserProfileResult { IsSuccess = false, ErrorMessage = "Invalid response format" };
            }

            var errorResult = JsonSerializer.Deserialize<UserProfileResult>(responseContent, _jsonOptions);
            return errorResult ?? new UserProfileResult { IsSuccess = false, ErrorMessage = "Failed to get profile" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile");
            return new UserProfileResult { IsSuccess = false, ErrorMessage = "An error occurred getting profile" };
        }
    }

    /// <inheritdoc />
    public Task<AuthenticationResult> SocialLoginAsync(SocialLoginRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Social login requested for provider {Provider}", request.Provider);
        throw new NotImplementedException("Social login will be implemented when first consumer requires it");
    }

    /// <inheritdoc />
    public Task<EmailVerificationResult> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Email verification requested for token {Token}", request.Token);
        throw new NotImplementedException("Email verification will be implemented when first consumer requires it");
    }

    /// <inheritdoc />
    public Task<EmailVerificationResult> ResendVerificationAsync(ResendVerificationRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Resend email verification requested for {Email}", request.Email);
        throw new NotImplementedException("Email verification resend will be implemented when first consumer requires it");
    }

    /// <inheritdoc />
    public Task<PasswordChangeResult> InitiatePasswordResetAsync(string email, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Password reset initiated for {Email}", email);
        throw new NotImplementedException("Password reset initiation will be implemented when first consumer requires it");
    }

    /// <inheritdoc />
    public Task<PasswordChangeResult> CompletePasswordResetAsync(string token, string newPassword, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Password reset completion requested for token {Token}", token);
        throw new NotImplementedException("Password reset completion will be implemented when first consumer requires it");
    }
}