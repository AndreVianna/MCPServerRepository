using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace MCPHub.WebApp.Services;

/// <summary>
/// Custom authentication state provider for Blazor Server with JWT tokens
/// </summary>
public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<CustomAuthenticationStateProvider> _logger;
    private ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(IJSRuntime jsRuntime, ILogger<CustomAuthenticationStateProvider> logger)
    {
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    /// <summary>
    /// Gets the current authentication state
    /// </summary>
    /// <returns>Authentication state</returns>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await GetTokenFromStorageAsync();
            
            if (string.IsNullOrEmpty(token))
            {
                return new AuthenticationState(_anonymous);
            }

            var claimsPrincipal = CreateClaimsPrincipalFromToken(token);
            
            if (claimsPrincipal == null || IsTokenExpired(token))
            {
                await ClearTokenAsync();
                return new AuthenticationState(_anonymous);
            }

            return new AuthenticationState(claimsPrincipal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting authentication state");
            return new AuthenticationState(_anonymous);
        }
    }

    /// <summary>
    /// Marks the user as authenticated
    /// </summary>
    /// <param name="token">JWT access token</param>
    /// <param name="refreshToken">Refresh token</param>
    public async Task MarkUserAsAuthenticatedAsync(string token, string refreshToken)
    {
        try
        {
            await SetTokenInStorageAsync(token, refreshToken);
            
            var claimsPrincipal = CreateClaimsPrincipalFromToken(token);
            if (claimsPrincipal != null)
            {
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking user as authenticated");
        }
    }

    /// <summary>
    /// Marks the user as logged out
    /// </summary>
    public async Task MarkUserAsLoggedOutAsync()
    {
        try
        {
            await ClearTokenAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking user as logged out");
        }
    }

    /// <summary>
    /// Gets the stored access token
    /// </summary>
    /// <returns>Access token</returns>
    public async Task<string?> GetTokenAsync()
    {
        return await GetTokenFromStorageAsync();
    }

    /// <summary>
    /// Gets the stored refresh token
    /// </summary>
    /// <returns>Refresh token</returns>
    public async Task<string?> GetRefreshTokenAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "refreshToken");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting refresh token from storage");
            return null;
        }
    }

    private async Task<string?> GetTokenFromStorageAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "accessToken");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting token from storage");
            return null;
        }
    }

    private async Task SetTokenInStorageAsync(string token, string refreshToken)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "accessToken", token);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "refreshToken", refreshToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting tokens in storage");
        }
    }

    private async Task ClearTokenAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "accessToken");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "refreshToken");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing tokens from storage");
        }
    }

    private static ClaimsPrincipal? CreateClaimsPrincipalFromToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            
            var claims = jsonToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "jwt");
            
            return new ClaimsPrincipal(identity);
        }
        catch
        {
            return null;
        }
    }

    private static bool IsTokenExpired(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            
            return jsonToken.ValidTo < DateTime.UtcNow;
        }
        catch
        {
            return true;
        }
    }
}