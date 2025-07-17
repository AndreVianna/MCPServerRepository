namespace Domain.Entities;

/// <summary>
/// Represents an MCP server in the registry
/// </summary>
public class Server
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid PublisherId { get; private set; }
    public Publisher Publisher { get; private set; } = null!;
    public string? Repository { get; private set; }
    public string? License { get; private set; }
    public List<string> Tags { get; private set; } = new();
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public ServerStatus Status { get; private set; }
    public TrustTier TrustTier { get; private set; }
    public List<ServerVersion> Versions { get; private set; } = new();

    private Server() { } // For EF Core

    public Server(
        string name,
        string description,
        Guid publisherId,
        string? repository = null,
        string? license = null,
        List<string>? tags = null)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        PublisherId = publisherId;
        Repository = repository;
        License = license;
        Tags = tags ?? new List<string>();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Status = ServerStatus.Pending;
        TrustTier = TrustTier.Unverified;
    }

    public void UpdateStatus(ServerStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTrustTier(TrustTier trustTier)
    {
        TrustTier = trustTier;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string description, string? repository, string? license, List<string>? tags)
    {
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Repository = repository;
        License = license;
        Tags = tags ?? new List<string>();
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum ServerStatus
{
    Pending,
    Approved,
    Rejected,
    Suspended
}