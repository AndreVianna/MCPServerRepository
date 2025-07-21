using System.Diagnostics.CodeAnalysis;

using MCPHub.Domain.Common;

namespace MCPHub.Domain.Entities;

/// <summary>
/// Represents an MCP server in the registry
/// </summary>
public class Server : BaseEntity {
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid PublisherId { get; set; }
    public Publisher Publisher { get; set; } = null!;
    public string? Repository { get; set; }
    public string? License { get; set; }
    public List<string> Tags { get; set; } = [];
    public ServerStatus Status { get; set; } = ServerStatus.Pending;
    public TrustTier TrustTier { get; set; } = TrustTier.Unverified;
    public List<ServerVersion> Versions { get; set; } = [];

    private Server() { } // For EF Core

    [SetsRequiredMembers]
    public Server(
        string name,
        string description,
        Guid publisherId,
        string? repository = null,
        string? license = null,
        List<string>? tags = null) {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        Name = name;
        Description = description;
        PublisherId = publisherId;
        Repository = repository;
        License = license;
        Tags = tags ?? [];
        Status = ServerStatus.Pending;
        TrustTier = TrustTier.Unverified;
        AuditTrail.Add(new AuditEntry {
            Action = "Created",
            UserId = Guid.Empty, // System action
            DateTime = DateTimeOffset.UtcNow
        });
    }

    public void UpdateStatus(ServerStatus status) {
        Status = status;
        AuditTrail.Add(new AuditEntry {
            Action = $"Status Updated to {status}",
            UserId = Guid.Empty, // System action
            DateTime = DateTimeOffset.UtcNow
        });
    }

    public void UpdateTrustTier(TrustTier trustTier) {
        TrustTier = trustTier;
        AuditTrail.Add(new AuditEntry {
            Action = $"Trust Tier Updated to {trustTier}",
            UserId = Guid.Empty, // System action
            DateTime = DateTimeOffset.UtcNow
        });
    }

    public void UpdateDetails(string description, string? repository, string? license, List<string>? tags) {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        Description = description;
        Repository = repository;
        License = license;
        Tags = tags ?? [];
        AuditTrail.Add(new AuditEntry {
            Action = "Details Updated",
            UserId = Guid.Empty, // System action
            DateTime = DateTimeOffset.UtcNow
        });
    }
}