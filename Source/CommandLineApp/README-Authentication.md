# CLI Authentication System Implementation

This document provides a comprehensive overview of the secure CLI authentication system implemented for MCPM (MCP Package Manager).

## Overview

The authentication system provides secure, cross-platform credential storage and management for CLI operations that require authentication (publishing, private package access, etc.). It follows the Contracts First architectural principle with platform-specific implementations.

## Architecture

### Core Interfaces

#### ICredentialStore
```csharp
public interface ICredentialStore
{
    Task<bool> StoreTokenAsync(string registryUrl, string token, string username, CancellationToken cancellationToken = default);
    Task<StoredCredential?> GetTokenAsync(string registryUrl, CancellationToken cancellationToken = default);
    Task<bool> RemoveTokenAsync(string registryUrl, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> ListStoredRegistriesAsync(CancellationToken cancellationToken = default);
    Task<bool> ClearAllAsync(CancellationToken cancellationToken = default);
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
    string StorageMechanism { get; }
    bool IsSecure { get; }
}
```

#### IAuthenticationManager
```csharp
public interface IAuthenticationManager
{
    Task<AuthenticationResult> LoginAsync(string registryUrl, string username, string password, CancellationToken cancellationToken = default);
    Task<AuthenticationResult> LoginWithApiKeyAsync(string registryUrl, string apiKey, CancellationToken cancellationToken = default);
    Task<bool> LogoutAsync(string registryUrl, CancellationToken cancellationToken = default);
    Task<CurrentUserInfo?> GetCurrentUserAsync(string registryUrl, CancellationToken cancellationToken = default);
    Task<TokenValidationResult> ValidateTokenAsync(string registryUrl, CancellationToken cancellationToken = default);
    Task<TokenRefreshResult> RefreshTokenAsync(string registryUrl, CancellationToken cancellationToken = default);
    Task<string?> GetAuthenticationHeaderAsync(string registryUrl, CancellationToken cancellationToken = default);
    // ... additional methods
}
```

### Platform-Specific Implementations

The system includes secure credential storage implementations for all major platforms:

1. **Windows**: `WindowsCredentialStore` - Uses Windows Credential Manager
2. **macOS**: `MacOSKeychainStore` - Uses macOS Keychain Services
3. **Linux**: `LinuxCredentialStore` - Uses libsecret with encrypted file fallback
4. **Fallback**: `FallbackCredentialStore` - Encrypted local file storage

### Factory Pattern

The `CredentialStoreFactory` automatically selects the best available credential store for the current platform:

```csharp
public interface ICredentialStoreFactory
{
    Task<ICredentialStore> CreateCredentialStoreAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ICredentialStore>> GetAvailableStoresAsync(CancellationToken cancellationToken = default);
    Task<CredentialStorePlatformInfo> GetPlatformInfoAsync(CancellationToken cancellationToken = default);
}
```

## Configuration Enhancement

### Multiple Registry Support

The `McpmConfiguration` has been enhanced to support multiple registries:

```csharp
public class McpmConfiguration
{
    // Default registry (backward compatibility)
    public RegistryConfiguration Registry { get; set; } = new();
    
    // Multiple registry configurations
    public Dictionary<string, RegistryConfiguration> Registries { get; set; } = new();
    
    // Deprecated auth config (use ICredentialStore instead)
    [Obsolete("Use ICredentialStore for secure authentication storage")]
    public AuthConfiguration Auth { get; set; } = new();
}
```

### Enhanced Registry Configuration

```csharp
public class RegistryConfiguration
{
    public string Url { get; set; } = "https://api.mcphub.dev";
    public string? Name { get; set; }
    public bool RequiresAuthentication { get; set; } = false;
    public bool SupportsPrivatePackages { get; set; } = true;
    public bool SupportsPublishing { get; set; } = true;
    public string? AuthEndpoint { get; set; }
    public IList<string> SupportedAuthMethods { get; set; } = new List<string> { "bearer", "api-key" };
    public bool VerifySSL { get; set; } = true;
    public Dictionary<string, string> Headers { get; set; } = new();
}
```

## CLI Commands

### Authentication Commands

The system includes a comprehensive `auth` command with subcommands:

```bash
# Login with API key
mcpm auth login --api-key YOUR_API_KEY

# Login with username (interactive password prompt)
mcpm auth login --username your-username --interactive

# Login to specific registry
mcpm auth login --registry https://custom-registry.com --api-key YOUR_KEY

# Show current authentication status
mcpm auth whoami

# Show all authenticated registries
mcpm auth list

# Show credential storage information
mcpm auth info

# Logout from default registry
mcpm auth logout

# Logout from specific registry
mcpm auth logout --registry https://custom-registry.com

# Logout from all registries
mcpm auth logout --all
```

## Implementation Status

### Phase 1: Contracts and Infrastructure ✅ COMPLETED

1. ✅ **ICredentialStore interface** - Secure token storage abstraction
2. ✅ **IAuthenticationManager interface** - Token management and validation
3. ✅ **Platform-specific credential stores** - Windows, macOS, Linux implementations (skeletons)
4. ✅ **AuthenticationManager skeleton** - NotImplementedException placeholders
5. ✅ **CredentialStoreFactory** - Platform detection and store selection
6. ✅ **Enhanced McpmConfiguration** - Multiple registry support
7. ✅ **DI container registration** - Service registration and wiring
8. ✅ **AuthCommand** - Complete CLI command implementation

### Phase 2: Implementation (When First Consumer Requires) 🔄 PENDING

1. 🔄 **Windows Credential Manager store** - WinAPI integration
2. 🔄 **macOS Keychain store** - Security framework integration
3. 🔄 **Linux libsecret store** - libsecret with encrypted file fallback
4. 🔄 **Token validation and refresh logic** - JWT handling and API validation
5. 🔄 **Authentication manager core functionality** - Complete implementation

## Security Features

### Secure Storage Mechanisms

- **Windows**: Uses Windows Credential Manager with user-specific encryption
- **macOS**: Uses Keychain Services with keychain item protection
- **Linux**: Attempts libsecret (GNOME Keyring/KDE Wallet), falls back to AES-256 encrypted files
- **Fallback**: AES-256 encrypted JSON files with salt and IV

### Token Management

- **Token Validation**: Validates tokens before API requests
- **Automatic Refresh**: Attempts token refresh when expired
- **Multiple Registry Support**: Separate authentication per registry
- **Secure Headers**: Proper Bearer token authentication headers

### Security Best Practices

- **No Plaintext Storage**: All tokens stored with platform-specific encryption
- **Metadata Tracking**: Stores creation time, expiration, and user information
- **Secure Cleanup**: Provides methods to clear all stored credentials
- **Platform Detection**: Automatically selects most secure available storage

## Integration Examples

### Using Authentication in Commands

```csharp
public class PublishCommand : BaseCommand
{
    private readonly IAuthenticationManager _authManager;
    
    public async Task<int> ExecuteAsync(PublishOptions options)
    {
        // Check authentication before publishing
        var isAuthenticated = await _authManager.IsAuthenticatedAsync(registryUrl);
        if (!isAuthenticated)
        {
            OutputFormatter.WriteError("Authentication required. Run 'mcpm auth login' first.");
            return 401;
        }
        
        // Get authentication header for API requests
        var authHeader = await _authManager.GetAuthenticationHeaderAsync(registryUrl);
        // Use authHeader in HTTP requests...
    }
}
```

### Updating API Client Integration

```csharp
public class McpHubApiClient : IMcpHubApiClient
{
    private readonly IAuthenticationManager _authManager;
    
    private async Task<HttpRequestMessage> CreateAuthenticatedRequestAsync(string registryUrl, HttpMethod method, string endpoint)
    {
        var request = new HttpRequestMessage(method, endpoint);
        
        var authHeader = await _authManager.GetAuthenticationHeaderAsync(registryUrl);
        if (!string.IsNullOrEmpty(authHeader))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authHeader);
        }
        
        return request;
    }
}
```

## Error Handling

The system provides comprehensive error handling:

- **Network Errors**: Graceful handling of connectivity issues
- **Authentication Failures**: Clear error messages for auth problems
- **Token Expiration**: Automatic refresh attempts with fallback to re-authentication
- **Platform Limitations**: Fallback to less secure but functional storage when needed

## Testing Strategy

### Unit Tests (To Be Implemented)

```csharp
[Test]
public async Task CredentialStore_StoreAndRetrieve_Success()
{
    // Test credential storage and retrieval
}

[Test] 
public async Task AuthenticationManager_ValidateExpiredToken_RefreshesAutomatically()
{
    // Test token refresh logic
}

[Test]
public async Task CredentialStoreFactory_SelectsBestAvailableStore()
{
    // Test platform-specific store selection
}
```

### Integration Tests

- Test authentication flow end-to-end
- Verify platform-specific storage mechanisms
- Test multiple registry configurations
- Validate error handling scenarios

## Future Enhancements

1. **OAuth 2.0 Support**: Device flow for web-based authentication
2. **MFA Support**: Multi-factor authentication integration
3. **Session Management**: Session timeout and renewal
4. **Audit Logging**: Track authentication events
5. **Enterprise SSO**: SAML/OIDC integration for enterprise environments

## Migration Guide

### From Legacy Auth Configuration

The old `McpmConfiguration.Auth` property is now obsolete. To migrate:

1. **Remove** any direct usage of `Configuration.Auth.Token`
2. **Use** `IAuthenticationManager.GetAuthenticationHeaderAsync()` instead
3. **Run** `mcpm auth login` to establish secure credential storage
4. **Update** any scripts to use the new auth commands

This implementation provides a solid foundation for secure CLI authentication while maintaining the flexibility to add new authentication methods and platforms in the future.