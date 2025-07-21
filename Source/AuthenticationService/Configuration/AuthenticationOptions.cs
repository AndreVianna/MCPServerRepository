namespace MCPHub.AuthenticationService.Configuration;

/// <summary>
/// Authentication configuration options
/// </summary>
public class AuthenticationOptions {
    public const string SectionName = "Authentication";

    /// <summary>
    /// Whether email confirmation is required for new users
    /// </summary>
    public bool RequireEmailConfirmation { get; set; } = false;

    /// <summary>
    /// Whether user registration is allowed
    /// </summary>
    public bool AllowUserRegistration { get; set; } = true;

    /// <summary>
    /// Default role assigned to new users
    /// </summary>
    public string DefaultUserRole { get; set; } = "User";
}