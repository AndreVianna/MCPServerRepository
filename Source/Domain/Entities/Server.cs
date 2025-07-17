namespace Domain.Entities;

/// <summary>
/// Represents an MCP server in the registry
/// </summary>
public class Server {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid PublisherId { get; set; }
    public Publisher Publisher { get; set; } = null!;
    public string? Repository { get; set; }
    public string? License { get; set; }
    public List<string> Tags { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ServerStatus Status { get; set; }
    public TrustTier TrustTier { get; set; }
    public List<ServerVersion> Versions { get; set; } = [];

    private Server() { } // For EF Core

    public Server(
        string name,
        string description,
        Guid publisherId,
        string? repository = null,
        string? license = null,
        List<string>? tags = null) {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        PublisherId = publisherId;
        Repository = repository;
        License = license;
        Tags = tags ?? [];
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Status = ServerStatus.Pending;
        TrustTier = TrustTier.Unverified;
    }

    public void UpdateStatus(ServerStatus status) {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTrustTier(TrustTier trustTier) {
        TrustTier = trustTier;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string description, string? repository, string? license, List<string>? tags) {
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Repository = repository;
        License = license;
        Tags = tags ?? [];
        UpdatedAt = DateTime.UtcNow;
    }
}