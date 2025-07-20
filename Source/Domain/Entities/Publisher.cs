using System.Diagnostics.CodeAnalysis;

using MCPHub.Domain.Common;

namespace MCPHub.Domain.Entities;

/// <summary>
/// Represents a package publisher in the MCP registry
/// </summary>
public class Publisher : BaseEntity {
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? OrganizationName { get; set; }
    public string? Website { get; set; }
    public bool Verified { get; set; } = false;
    public PublisherType Type { get; set; }
    public List<Server> Servers { get; set; } = [];
    public List<Package> Packages { get; set; } = [];

    private Publisher() { } // For EF Core

    [SetsRequiredMembers]
    public Publisher(
        string name,
        string email,
        PublisherType type,
        string? organizationName = null,
        string? website = null) {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        Name = name;
        Email = email;
        Type = type;
        OrganizationName = organizationName;
        Website = website;
        Verified = false;
    }

    public void Verify() {
        Verified = true;
        SetUpdatedBy(UpdatedBy ?? "system");
    }

    public void UpdateDetails(string? organizationName, string? website) {
        OrganizationName = organizationName;
        Website = website;
        SetUpdatedBy(UpdatedBy ?? "system");
    }
}