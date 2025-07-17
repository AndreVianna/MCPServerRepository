namespace Domain.Entities;

/// <summary>
/// Represents a package publisher in the MCP registry
/// </summary>
public class Publisher {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? OrganizationName { get; set; }
    public string? Website { get; set; }
    public bool Verified { get; set; }
    public PublisherType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Server> Servers { get; set; } = [];
    public List<Package> Packages { get; set; } = [];

    private Publisher() { } // For EF Core

    public Publisher(
        string name,
        string email,
        PublisherType type,
        string? organizationName = null,
        string? website = null) {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Type = type;
        OrganizationName = organizationName;
        Website = website;
        Verified = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Verify() {
        Verified = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string? organizationName, string? website) {
        OrganizationName = organizationName;
        Website = website;
        UpdatedAt = DateTime.UtcNow;
    }
}