namespace Domain.Entities;

/// <summary>
/// Represents a package publisher in the MCP registry
/// </summary>
public class Publisher
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string? OrganizationName { get; private set; }
    public string? Website { get; private set; }
    public bool Verified { get; private set; }
    public PublisherType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public List<Server> Servers { get; private set; } = new();
    public List<Package> Packages { get; private set; } = new();

    private Publisher() { } // For EF Core

    public Publisher(
        string name,
        string email,
        PublisherType type,
        string? organizationName = null,
        string? website = null)
    {
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

    public void Verify()
    {
        Verified = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string? organizationName, string? website)
    {
        OrganizationName = organizationName;
        Website = website;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum PublisherType
{
    Individual,
    Organization,
    Enterprise
}