using System.Text.Json.Serialization;

namespace MCPHub.Domain.ValueObjects;

/// <summary>
/// Represents the structure of an MCP package manifest (mcp-manifest.json)
/// </summary>
public class MCPManifest {
    /// <summary>
    /// Package name (must follow naming convention)
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Semantic version string
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Package description
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Package author information
    /// </summary>
    [JsonPropertyName("author")]
    public MCPAuthor Author { get; set; } = new();

    /// <summary>
    /// Package license (SPDX identifier)
    /// </summary>
    [JsonPropertyName("license")]
    public string License { get; set; } = string.Empty;

    /// <summary>
    /// Homepage URL
    /// </summary>
    [JsonPropertyName("homepage")]
    public string? Homepage { get; set; }

    /// <summary>
    /// Repository URL
    /// </summary>
    [JsonPropertyName("repository")]
    public string? Repository { get; set; }

    /// <summary>
    /// Bug tracker URL
    /// </summary>
    [JsonPropertyName("bugs")]
    public string? Bugs { get; set; }

    /// <summary>
    /// Package keywords/tags
    /// </summary>
    [JsonPropertyName("keywords")]
    public IEnumerable<string> Keywords { get; set; } = [];

    /// <summary>
    /// MCP capabilities this package provides
    /// </summary>
    [JsonPropertyName("capabilities")]
    public MCPCapabilities Capabilities { get; set; } = new();

    /// <summary>
    /// Required permissions for this package
    /// </summary>
    [JsonPropertyName("permissions")]
    public MCPPermissions Permissions { get; set; } = new();

    /// <summary>
    /// Runtime configuration for Docker containers
    /// </summary>
    [JsonPropertyName("runtime")]
    public MCPRuntime? Runtime { get; set; }

    /// <summary>
    /// Package dependencies
    /// </summary>
    [JsonPropertyName("dependencies")]
    public Dictionary<string, string> Dependencies { get; set; } = new();

    /// <summary>
    /// Development dependencies
    /// </summary>
    [JsonPropertyName("devDependencies")]
    public Dictionary<string, string> DevDependencies { get; set; } = new();

    /// <summary>
    /// Entry point for the MCP server
    /// </summary>
    [JsonPropertyName("main")]
    public string? Main { get; set; }

    /// <summary>
    /// Binary commands provided by this package
    /// </summary>
    [JsonPropertyName("bin")]
    public Dictionary<string, string> Bin { get; set; } = new();

    /// <summary>
    /// Files to include in the package
    /// </summary>
    [JsonPropertyName("files")]
    public IEnumerable<string> Files { get; set; } = [];

    // Convenience properties for easier access
    /// <summary>
    /// Server information (convenience property)
    /// </summary>
    [JsonIgnore]
    public ServerInfo Info => new() { Name = Name, Version = Version, Description = Description };

    /// <summary>
    /// Tools provided by this package (convenience property)
    /// </summary>
    [JsonIgnore]
    public IEnumerable<MCPTool> Tools => Capabilities.Tools;

    /// <summary>
    /// Resources provided by this package (convenience property)
    /// </summary>
    [JsonIgnore]
    public IEnumerable<MCPResource> Resources => Capabilities.Resources;

    /// <summary>
    /// Prompts provided by this package (convenience property)
    /// </summary>
    [JsonIgnore]
    public IEnumerable<MCPPrompt> Prompts => Capabilities.Prompts;

    /// <summary>
    /// Validates the manifest structure and required fields
    /// </summary>
    public ValidationSummary Validate() {
        var errors = new List<string>();
        var warnings = new List<string>();

        // Required fields validation
        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Package name is required");

        if (string.IsNullOrWhiteSpace(Version))
            errors.Add("Package version is required");

        if (string.IsNullOrWhiteSpace(Description))
            errors.Add("Package description is required");

        if (string.IsNullOrWhiteSpace(License))
            errors.Add("Package license is required");

        // Author validation
        if (string.IsNullOrWhiteSpace(Author.Name))
            errors.Add("Author name is required");

        // Name format validation (namespace/package or simple name)
        if (!string.IsNullOrWhiteSpace(Name)) {
            if (!IsValidPackageName(Name))
                errors.Add("Package name must contain only lowercase letters, numbers, hyphens, and optionally a namespace prefix (e.g., @namespace/package)");
        }

        // Version format validation (semantic versioning)
        if (!string.IsNullOrWhiteSpace(Version)) {
            if (!IsValidSemanticVersion(Version))
                errors.Add("Package version must follow semantic versioning (e.g., 1.0.0, 2.1.0-beta.1)");
        }

        // Capabilities validation
        if (!Capabilities.HasAnyCapability())
            warnings.Add("Package should provide at least one MCP capability (tools, resources, or prompts)");

        // URL validation
        if (!string.IsNullOrWhiteSpace(Homepage) && !IsValidUrl(Homepage))
            errors.Add("Homepage must be a valid URL");

        if (!string.IsNullOrWhiteSpace(Repository) && !IsValidUrl(Repository))
            errors.Add("Repository must be a valid URL");

        if (!string.IsNullOrWhiteSpace(Bugs) && !IsValidUrl(Bugs))
            errors.Add("Bugs URL must be a valid URL");

        return errors.Count == 0
            ? ValidationSummary.Success("manifest", 0, warnings)
            : ValidationSummary.Failure(errors, "manifest", 0, warnings);
    }

    private static bool IsValidPackageName(string name) {
        // Allow @namespace/package format or simple package name
        if (name.StartsWith('@')) {
            var parts = name.Split('/');
            if (parts.Length != 2)
                return false;
            return IsValidNamePart(parts[0][1..]) && IsValidNamePart(parts[1]);
        }

        return IsValidNamePart(name);
    }

    private static bool IsValidNamePart(string part) {
        if (string.IsNullOrWhiteSpace(part))
            return false;
        if (part.Length > 50)
            return false; // Reasonable length limit

        return part.All(c => char.IsLetterOrDigit(c) || c == '-') &&
               char.IsLetter(part[0]) &&
               !part.EndsWith('-');
    }

    private static bool IsValidSemanticVersion(string version) {
        try {
            var semVer = System.Version.Parse(version.Split('-')[0]);
            return semVer.Major >= 0 && semVer.Minor >= 0 && semVer.Build >= 0;
        }
        catch {
            return false;
        }
    }

    private static bool IsValidUrl(string url) => Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
               (uri.Scheme == "http" || uri.Scheme == "https");
}

/// <summary>
/// Author information in the manifest
/// </summary>
public class MCPAuthor {
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// MCP capabilities that this package provides
/// </summary>
public class MCPCapabilities {
    /// <summary>
    /// Tools/functions that this MCP server provides
    /// </summary>
    [JsonPropertyName("tools")]
    public IEnumerable<MCPTool> Tools { get; set; } = [];

    /// <summary>
    /// Resources that this MCP server can provide
    /// </summary>
    [JsonPropertyName("resources")]
    public IEnumerable<MCPResource> Resources { get; set; } = [];

    /// <summary>
    /// Prompts that this MCP server provides
    /// </summary>
    [JsonPropertyName("prompts")]
    public IEnumerable<MCPPrompt> Prompts { get; set; } = [];

    /// <summary>
    /// Logging capability
    /// </summary>
    [JsonPropertyName("logging")]
    public bool Logging { get; set; } = false;

    /// <summary>
    /// Checks if any capabilities are defined
    /// </summary>
    public bool HasAnyCapability() => Tools.Any() || Resources.Any() || Prompts.Any();
}

/// <summary>
/// MCP tool definition
/// </summary>
public class MCPTool {
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("inputSchema")]
    public object? InputSchema { get; set; }
}

/// <summary>
/// MCP resource definition
/// </summary>
public class MCPResource {
    [JsonPropertyName("uri")]
    public string Uri { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("mimeType")]
    public string? MimeType { get; set; }
}

/// <summary>
/// MCP prompt definition
/// </summary>
public class MCPPrompt {
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("arguments")]
    public IEnumerable<MCPPromptArgument> Arguments { get; set; } = [];
}

/// <summary>
/// MCP prompt argument definition
/// </summary>
public class MCPPromptArgument {
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("required")]
    public bool Required { get; set; } = false;
}

/// <summary>
/// Required permissions for the MCP package
/// </summary>
public class MCPPermissions {
    /// <summary>
    /// Network access permissions
    /// </summary>
    [JsonPropertyName("network")]
    public MCPNetworkPermissions? Network { get; set; }

    /// <summary>
    /// Filesystem access permissions
    /// </summary>
    [JsonPropertyName("filesystem")]
    public MCPFilesystemPermissions? Filesystem { get; set; }

    /// <summary>
    /// Environment variable access
    /// </summary>
    [JsonPropertyName("environment")]
    public IEnumerable<string> Environment { get; set; } = [];
}

/// <summary>
/// Network access permissions
/// </summary>
public class MCPNetworkPermissions {
    [JsonPropertyName("allowedHosts")]
    public IEnumerable<string> AllowedHosts { get; set; } = [];

    [JsonPropertyName("allowAll")]
    public bool AllowAll { get; set; } = false;
}

/// <summary>
/// Filesystem access permissions
/// </summary>
public class MCPFilesystemPermissions {
    [JsonPropertyName("allowedPaths")]
    public IEnumerable<string> AllowedPaths { get; set; } = [];

    [JsonPropertyName("readOnly")]
    public bool ReadOnly { get; set; } = true;
}

/// <summary>
/// Runtime configuration for Docker containers
/// </summary>
public class MCPRuntime {
    [JsonPropertyName("docker")]
    public MCPDockerConfig? Docker { get; set; }
}

/// <summary>
/// Docker configuration
/// </summary>
public class MCPDockerConfig {
    [JsonPropertyName("image")]
    public string Image { get; set; } = string.Empty;

    [JsonPropertyName("tag")]
    public string Tag { get; set; } = "latest";

    [JsonPropertyName("env")]
    public Dictionary<string, string> Environment { get; set; } = new();

    [JsonPropertyName("ports")]
    public IEnumerable<string> Ports { get; set; } = [];

    [JsonPropertyName("volumes")]
    public IEnumerable<string> Volumes { get; set; } = [];
}

/// <summary>
/// Server information structure
/// </summary>
public class ServerInfo
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Server capabilities structure
/// </summary>
public class ServerCapabilities
{
    public bool Logging { get; set; } = false;
    public bool Tools { get; set; } = false;
    public bool Resources { get; set; } = false;
    public bool Prompts { get; set; } = false;
}