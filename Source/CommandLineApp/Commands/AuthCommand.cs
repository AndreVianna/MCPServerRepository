using AuthenticationResult = MCPHub.CommandLineApp.Services.AuthenticationResult;

namespace MCPHub.CommandLineApp.Commands;

/// <summary>
/// Command for managing authentication with registries
/// </summary>
public class AuthCommand(
    ILogger<AuthCommand> logger,
    McpmConfiguration configuration,
    IMcpHubApiClient apiClient,
    IOutputFormatter outputFormatter,
    IInteractionService interactionService,
    IProgressReporter progressReporter,
    IAuthenticationManager authManager,
    ICredentialStoreFactory credentialStoreFactory) : BaseCommand(logger, configuration, apiClient, outputFormatter, interactionService, progressReporter) {
    private readonly IAuthenticationManager _authManager = authManager ?? throw new ArgumentNullException(nameof(authManager));
    private readonly ICredentialStoreFactory _credentialStoreFactory = credentialStoreFactory ?? throw new ArgumentNullException(nameof(credentialStoreFactory));

    /// <summary>
    /// Creates the auth command with subcommands
    /// </summary>
    /// <returns>The configured auth command</returns>
    public override Command CreateCommand() {
        var command = new Command("auth", "Manage authentication with package registries");

        // Add subcommands
        command.AddCommand(CreateLoginCommand());
        command.AddCommand(CreateLogoutCommand());
        command.AddCommand(CreateWhoAmICommand());
        command.AddCommand(CreateListCommand());
        command.AddCommand(CreateInfoCommand());

        return command;
    }

    private Command CreateLoginCommand() {
        var command = new Command("login", "Authenticate with a package registry");

        var registryOption = new Option<string?>(
            aliases: ["--registry", "-r"],
            description: "Registry URL to authenticate with (uses default if not specified)");

        var usernameOption = new Option<string?>(
            aliases: ["--username", "-u"],
            description: "Username for authentication");

        var apiKeyOption = new Option<string?>(
            aliases: ["--api-key", "-k"],
            description: "API key for authentication (alternative to username/password)");

        var interactiveOption = new Option<bool>(
            aliases: ["--interactive", "-i"],
            description: "Use interactive login (prompts for credentials)",
            getDefaultValue: () => true);

        command.AddOption(registryOption);
        command.AddOption(usernameOption);
        command.AddOption(apiKeyOption);
        command.AddOption(interactiveOption);

        command.SetHandler(LoginAsync, registryOption, usernameOption, apiKeyOption, interactiveOption);

        return command;
    }

    private Command CreateLogoutCommand() {
        var command = new Command("logout", "Remove authentication from a registry");

        var registryOption = new Option<string?>(
            aliases: ["--registry", "-r"],
            description: "Registry URL to logout from (uses default if not specified)");

        var allOption = new Option<bool>(
            aliases: ["--all", "-a"],
            description: "Logout from all registries");

        command.AddOption(registryOption);
        command.AddOption(allOption);

        command.SetHandler(LogoutAsync, registryOption, allOption);

        return command;
    }

    private Command CreateWhoAmICommand() {
        var command = new Command("whoami", "Show current authentication status");

        var registryOption = new Option<string?>(
            aliases: ["--registry", "-r"],
            description: "Registry URL to check (shows all if not specified)");

        command.AddOption(registryOption);

        command.SetHandler(WhoAmIAsync, registryOption);

        return command;
    }

    private Command CreateListCommand() {
        var command = new Command("list", "List all authenticated registries");

        command.SetHandler(ListAuthenticatedRegistriesAsync);

        return command;
    }

    private Command CreateInfoCommand() {
        var command = new Command("info", "Show information about credential storage");

        command.SetHandler(ShowCredentialStoreInfoAsync);

        return command;
    }

    private async Task LoginAsync(string? registryUrl, string? username, string? apiKey, bool interactive) {
        try {
            registryUrl ??= Configuration.DefaultRegistryUrl;

            using var loginProgress = ProgressReporter.CreateStepProgress("Authentication", [
                "Selecting authentication method",
                "Gathering credentials",
                "Authenticating with registry",
                "Storing credentials securely",
                                                                                                  ]);

            loginProgress.StartStep(0, "Determining authentication method...");

            AuthenticationResult result;

            if (!string.IsNullOrEmpty(apiKey)) {
                // Direct API key authentication
                loginProgress.CompleteStep(0, "Using provided API key");
                loginProgress.SkipStep(1, "API key provided");

                loginProgress.StartStep(2, $"Authenticating with {registryUrl}...");
                result = await _authManager.LoginWithApiKeyAsync(registryUrl, apiKey);
            }
            else if (!string.IsNullOrEmpty(username)) {
                // Username/password authentication
                loginProgress.CompleteStep(0, "Using username/password authentication");

                if (interactive) {
                    loginProgress.StartStep(1, "Prompting for password...");
                    var password = await InteractionService.PromptSecretAsync(
                        "Enter password:",
                        validator: pwd => string.IsNullOrEmpty(pwd) ? "Password is required" : null);

                    if (string.IsNullOrEmpty(password)) {
                        loginProgress.FailStep(1, "Password not provided");
                        OutputFormatter.WriteError("Password is required for username authentication");
                        return;
                    }

                    loginProgress.CompleteStep(1, "Password entered");
                    loginProgress.StartStep(2, $"Authenticating with {registryUrl}...");
                    // Mock implementation - replace with actual method
                    result = new AuthenticationResult {
                        IsSuccess = true,
                        UserInfo = new CurrentUserInfo { Username = username },
                    };
                }
                else {
                    loginProgress.FailStep(0, "Password required for username authentication");
                    OutputFormatter.WriteError("Password required for username authentication. Use --interactive or provide --api-key");
                    return;
                }
            }
            else if (interactive) {
                // Interactive mode - guided authentication
                loginProgress.CompleteStep(0, "Using interactive authentication");
                result = await HandleInteractiveLoginAsync(registryUrl, loginProgress);
            }
            else {
                loginProgress.FailStep(0, "No authentication method provided");
                OutputFormatter.WriteError("Authentication credentials required. Use --username, --api-key, or --interactive");
                return;
            }

            if (result.IsSuccess) {
                loginProgress.CompleteStep(2, "Authentication successful");
                loginProgress.StartStep(3, "Storing credentials...");

                // Credentials are already stored by the auth manager
                loginProgress.CompleteStep(3, "Credentials stored securely");

                await ShowLoginSuccessAsync(result, registryUrl);
            }
            else {
                loginProgress.FailStep(2, "Authentication failed");
                await ShowLoginFailureAsync(result);
            }
        }
        catch (Exception ex) {
            Logger.LogError(ex, "Failed to authenticate with registry");
            OutputFormatter.WriteError($"Authentication failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles interactive login with guided steps
    /// </summary>
    private async Task<AuthenticationResult> HandleInteractiveLoginAsync(string registryUrl, IStepProgress progress) {
        progress.StartStep(1, "Interactive authentication setup...");

        // Show available authentication methods
        var authMethods = new Dictionary<string, string> {
            { "apikey", "API Key (recommended for CI/CD)" },
            { "credentials", "Username and Password" },
            { "browser", "Browser-based OAuth (if supported)" },
                                                         };

        var selectedMethod = await InteractionService.ShowMenuAsync(
            "Select authentication method:", authMethods);

        switch (selectedMethod) {
            case "apikey":
                return await HandleApiKeyLoginAsync(registryUrl, progress);

            case "credentials":
                return await HandleCredentialsLoginAsync(registryUrl, progress);

            case "browser":
                return await HandleBrowserLoginAsync(registryUrl, progress);

            default:
                progress.FailStep(1, "No authentication method selected");
                return new AuthenticationResult { IsSuccess = false, ErrorMessage = "Authentication cancelled" };
        }
    }

    /// <summary>
    /// Handles API key login interactively
    /// </summary>
    private async Task<AuthenticationResult> HandleApiKeyLoginAsync(string registryUrl, IStepProgress progress) {
        progress.UpdateStatus("Setting up API key authentication...");

        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("API Key Authentication:");
        OutputFormatter.WriteInfo($"1. Visit {registryUrl}/settings/tokens");
        OutputFormatter.WriteInfo("2. Generate a new API token");
        OutputFormatter.WriteInfo("3. Copy the token and paste it below");
        OutputFormatter.WriteLine();

        var apiKey = await InteractionService.PromptSecretAsync(
            "Enter your API key:",
            validator: key => string.IsNullOrWhiteSpace(key) ? "API key is required" : null);

        if (string.IsNullOrEmpty(apiKey)) {
            progress.FailStep(1, "API key not provided");
            return new AuthenticationResult { IsSuccess = false, ErrorMessage = "API key is required" };
        }

        progress.CompleteStep(1, "API key entered");
        return await _authManager.LoginWithApiKeyAsync(registryUrl, apiKey);
    }

    /// <summary>
    /// Handles username/password login interactively
    /// </summary>
    private async Task<AuthenticationResult> HandleCredentialsLoginAsync(string registryUrl, IStepProgress progress) {
        progress.UpdateStatus("Setting up username/password authentication...");

        var username = await InteractionService.PromptAsync(
            "Enter username:",
            validator: user => string.IsNullOrWhiteSpace(user) ? "Username is required" : null);

        if (string.IsNullOrEmpty(username)) {
            progress.FailStep(1, "Username not provided");
            return new AuthenticationResult { IsSuccess = false, ErrorMessage = "Username is required" };
        }

        var password = await InteractionService.PromptSecretAsync(
            "Enter password:",
            validator: pwd => string.IsNullOrWhiteSpace(pwd) ? "Password is required" : null);

        if (string.IsNullOrEmpty(password)) {
            progress.FailStep(1, "Password not provided");
            return new AuthenticationResult { IsSuccess = false, ErrorMessage = "Password is required" };
        }

        progress.CompleteStep(1, "Credentials entered");
        // Mock implementation - replace with actual method
        return new AuthenticationResult {
            IsSuccess = true,
            UserInfo = new CurrentUserInfo { Username = username },
        };
    }

    /// <summary>
    /// Handles browser-based OAuth login
    /// </summary>
    private async Task<AuthenticationResult> HandleBrowserLoginAsync(string registryUrl, IStepProgress progress) {
        progress.UpdateStatus("Setting up browser authentication...");

        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("Browser Authentication:");
        OutputFormatter.WriteInfo("This will open your default browser for authentication.");

        var continueWithBrowser = await InteractionService.ConfirmAsync(
            "Continue with browser authentication?", true);

        if (!continueWithBrowser) {
            progress.FailStep(1, "Browser authentication cancelled");
            return new AuthenticationResult { IsSuccess = false, ErrorMessage = "Authentication cancelled" };
        }

        progress.CompleteStep(1, "Browser authentication initiated");

        // Placeholder for browser authentication
        OutputFormatter.WriteInfo("Browser authentication would be implemented here");
        OutputFormatter.WriteInfo("This would typically:");
        OutputFormatter.WriteInfo("1. Start local HTTP server");
        OutputFormatter.WriteInfo("2. Open browser to OAuth URL");
        OutputFormatter.WriteInfo("3. Handle OAuth callback");
        OutputFormatter.WriteInfo("4. Extract and store tokens");

        return new AuthenticationResult {
            IsSuccess = false,
            ErrorMessage = "Browser authentication not yet implemented",
        };
    }

    /// <summary>
    /// Shows login success with enhanced information
    /// </summary>
    private async Task ShowLoginSuccessAsync(AuthenticationResult result, string registryUrl) {
        OutputFormatter.WriteSuccess($"Successfully authenticated with {registryUrl}");

        if (result.UserInfo != null) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteInfo("Account Information:");
            OutputFormatter.WriteInfo($"  Username: {result.UserInfo.Username}");

            if (!string.IsNullOrEmpty(result.UserInfo.Email)) {
                OutputFormatter.WriteInfo($"  Email: {result.UserInfo.Email}");
            }

            if (!string.IsNullOrEmpty(result.UserInfo.DisplayName)) {
                OutputFormatter.WriteInfo($"  Display Name: {result.UserInfo.DisplayName}");
            }

            // Mock token expiration check
            var mockTokenExpiry = DateTimeOffset.UtcNow.AddDays(30);
            OutputFormatter.WriteInfo($"  Token Expires: {mockTokenExpiry:yyyy-MM-dd HH:mm:ss} UTC");
        }

        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("You can now:");
        OutputFormatter.WriteInfo("  • Search and install packages: mcpm search <query>");
        OutputFormatter.WriteInfo("  • Publish your own packages: mcpm publish");
        OutputFormatter.WriteInfo("  • View account info: mcpm auth whoami");

        // Show next steps for first-time users (mock check)
        var isFirstTime = result.UserInfo?.Username != null; // Mock logic
        if (isFirstTime) {
            var showGuidance = await InteractionService.ConfirmAsync(
                "Would you like to see getting started guidance?", true);

            if (showGuidance) {
                await ShowGettingStartedGuidanceAsync();
            }
        }
    }

    /// <summary>
    /// Shows login failure with helpful suggestions
    /// </summary>
    private async Task ShowLoginFailureAsync(AuthenticationResult result) {
        OutputFormatter.WriteError($"Authentication failed: {result.ErrorMessage}");

        if (!string.IsNullOrEmpty(result.ErrorDetails)) {
            OutputFormatter.WriteError($"Details: {result.ErrorDetails}");
        }

        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("Troubleshooting suggestions:");

        var errorLower = result.ErrorMessage?.ToLower() ?? "";
        if (errorLower.Contains("invalid") || errorLower.Contains("unauthorized")) {
            OutputFormatter.WriteInfo("  • Check that your credentials are correct");
            OutputFormatter.WriteInfo("  • Ensure your API key hasn't expired");
            OutputFormatter.WriteInfo("  • Verify you're using the correct registry URL");
        }
        else if (errorLower.Contains("network") || errorLower.Contains("timeout")) {
            OutputFormatter.WriteInfo("  • Check your internet connection");
            OutputFormatter.WriteInfo("  • Try again in a few moments");
            OutputFormatter.WriteInfo("  • Check if registry is accessible: " + Configuration.DefaultRegistryUrl);
        }
        else {
            OutputFormatter.WriteInfo("  • Try using --interactive for guided authentication");
            OutputFormatter.WriteInfo("  • Check registry documentation for authentication methods");
            OutputFormatter.WriteInfo("  • Contact registry support if issue persists");
        }

        var retryLogin = await InteractionService.ConfirmAsync("Would you like to try again?");
        if (retryLogin) {
            await LoginAsync(null, null, null, true);
        }
    }

    /// <summary>
    /// Shows getting started guidance for new users
    /// </summary>
    private async Task ShowGettingStartedGuidanceAsync() {
        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("Getting Started with MCP Hub:");
        OutputFormatter.WriteLine();

        OutputFormatter.WriteInfo("1. Discover Packages:");
        OutputFormatter.WriteInfo("   mcpm search <keyword>     # Search for packages");
        OutputFormatter.WriteInfo("   mcpm info <package>       # Get package details");
        OutputFormatter.WriteLine();

        OutputFormatter.WriteInfo("2. Install Packages:");
        OutputFormatter.WriteInfo("   mcpm install <package>    # Install a package");
        OutputFormatter.WriteInfo("   mcpm list                 # View installed packages");
        OutputFormatter.WriteLine();

        OutputFormatter.WriteInfo("3. Publish Your Own:");
        OutputFormatter.WriteInfo("   mcpm publish              # Publish from current directory");
        OutputFormatter.WriteInfo("   mcpm publish --dry-run    # Test publishing process");
        OutputFormatter.WriteLine();

        var viewExamples = await InteractionService.ConfirmAsync("View example commands?");
        if (viewExamples) {
            OutputFormatter.WriteInfo("Example Commands:");
            OutputFormatter.WriteInfo("  mcpm search \"file management\"");
            OutputFormatter.WriteInfo("  mcpm install awesome-mcp-server");
            OutputFormatter.WriteInfo("  mcpm auth whoami");
        }
    }

    private async Task LogoutAsync(string? registryUrl, bool all) {
        try {
            if (all) {
                var success = await _authManager.ClearAllAuthenticationAsync();
                if (success) {
                    OutputFormatter.WriteSuccess("Logged out from all registries");
                }
                else {
                    OutputFormatter.WriteError("Failed to logout from all registries");
                }
            }
            else {
                registryUrl ??= Configuration.DefaultRegistryUrl;
                var success = await _authManager.LogoutAsync(registryUrl);

                if (success) {
                    OutputFormatter.WriteSuccess($"Logged out from {registryUrl}");
                }
                else {
                    OutputFormatter.WriteError($"Failed to logout from {registryUrl}");
                }
            }
        }
        catch (Exception ex) {
            Logger.LogError(ex, "Failed to logout");
            OutputFormatter.WriteError($"Logout failed: {ex.Message}");
        }
    }

    private async Task WhoAmIAsync(string? registryUrl) {
        try {
            if (string.IsNullOrEmpty(registryUrl)) {
                // Show all authenticated registries
                var registries = await _authManager.GetAuthenticatedRegistriesAsync();
                if (!registries.Any()) {
                    OutputFormatter.WriteInfo("Not authenticated with any registries");
                    return;
                }

                OutputFormatter.WriteInfo("Authenticated registries:");
                foreach (var registry in registries) {
                    var status = registry.IsValid ? "✓" : "✗";
                    OutputFormatter.WriteInfo($"  {status} {registry.Url}: {registry.Username}");
                }
            }
            else {
                // Show specific registry
                var currentUser = await _authManager.GetCurrentUserAsync(registryUrl);
                if (currentUser != null) {
                    OutputFormatter.WriteInfo($"Registry: {registryUrl}");
                    OutputFormatter.WriteInfo($"Username: {currentUser.Username}");
                    if (!string.IsNullOrEmpty(currentUser.Email)) {
                        OutputFormatter.WriteInfo($"Email: {currentUser.Email}");
                    }
                    if (!string.IsNullOrEmpty(currentUser.DisplayName)) {
                        OutputFormatter.WriteInfo($"Display Name: {currentUser.DisplayName}");
                    }
                }
                else {
                    OutputFormatter.WriteInfo($"Not authenticated with {registryUrl}");
                }
            }
        }
        catch (Exception ex) {
            Logger.LogError(ex, "Failed to get authentication status");
            OutputFormatter.WriteError($"Failed to get authentication status: {ex.Message}");
        }
    }

    private async Task ListAuthenticatedRegistriesAsync() {
        try {
            var registries = await _authManager.GetAuthenticatedRegistriesAsync();

            if (!registries.Any()) {
                OutputFormatter.WriteInfo("No authenticated registries found");
                return;
            }

            OutputFormatter.WriteInfo($"Found {registries.Count()} authenticated registries:");

            foreach (var registry in registries.OrderBy(r => r.Url)) {
                var status = registry.IsValid ? "Valid" : "Invalid/Expired";
                OutputFormatter.WriteInfo($"  {registry.Url}");
                OutputFormatter.WriteInfo($"    User: {registry.Username}");
                OutputFormatter.WriteInfo($"    Status: {status}");
                OutputFormatter.WriteInfo($"    Authenticated: {registry.AuthenticatedAt:yyyy-MM-dd HH:mm:ss}");
                if (registry.ExpiresAt.HasValue) {
                    OutputFormatter.WriteInfo($"    Expires: {registry.ExpiresAt:yyyy-MM-dd HH:mm:ss}");
                }
            }
        }
        catch (Exception ex) {
            Logger.LogError(ex, "Failed to list authenticated registries");
            OutputFormatter.WriteError($"Failed to list registries: {ex.Message}");
        }
    }

    private async Task ShowCredentialStoreInfoAsync() {
        try {
            var platformInfo = await _credentialStoreFactory.GetPlatformInfoAsync();

            OutputFormatter.WriteInfo("Credential Storage Information:");
            OutputFormatter.WriteInfo($"Platform: {platformInfo.Platform}");
            OutputFormatter.WriteInfo($"Preferred Store: {platformInfo.PreferredStore}");
            OutputFormatter.WriteInfo($"Secure Storage Available: {(platformInfo.HasSecureStorage ? "Yes" : "No")}");

            if (!string.IsNullOrEmpty(platformInfo.Notes)) {
                OutputFormatter.WriteInfo($"Notes: {platformInfo.Notes}");
            }

            OutputFormatter.WriteInfo("\nAvailable Credential Stores:");
            foreach (var store in platformInfo.AvailableStores.OrderBy(s => s.PreferenceOrder)) {
                var available = store.IsAvailable ? "Available" : "Unavailable";
                var secure = store.IsSecure ? "Secure" : "Insecure";

                OutputFormatter.WriteInfo($"  {store.PreferenceOrder + 1}. {store.Name} ({available}, {secure})");
                if (!string.IsNullOrEmpty(store.Description)) {
                    OutputFormatter.WriteInfo($"     {store.Description}");
                }
            }
        }
        catch (Exception ex) {
            Logger.LogError(ex, "Failed to get credential store information");
            OutputFormatter.WriteError($"Failed to get store information: {ex.Message}");
        }
    }
}