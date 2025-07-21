using System.Diagnostics.CodeAnalysis;

using MCPHub.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace MCPHub.Domain.Entities;

/// <summary>
/// Represents an application user extending ASP.NET Core Identity
/// </summary>
public class ApplicationUser : IdentityUser<Guid> {
    public override Guid Id { get; set; } = Guid.CreateVersion7();
    public string? DisplayName { get; set; }
    public string? GitHubUsername { get; set; }
    public string? TwitterHandle { get; set; }
    public string? Website { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public ICollection<IAuditEntry> AuditTrail { get; } = [];
    public bool IsEmailVerified { get; set; } = false;
    public bool IsPublisher { get; set; } = false;
    public List<Publisher> Publishers { get; set; } = [];

    private ApplicationUser() { } // For EF Core

    [SetsRequiredMembers]
    public ApplicationUser(string userName, string email) {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        UserName = userName;
        Email = email;
        AuditTrail.Add(new AuditEntry {
            Action = "Created",
            UserId = Id,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    public void UpdateProfile(
        string? displayName = null,
        string? gitHubUsername = null,
        string? twitterHandle = null,
        string? website = null,
        string? bio = null,
        string? avatarUrl = null) {
        DisplayName = displayName;
        GitHubUsername = gitHubUsername;
        TwitterHandle = twitterHandle;
        Website = website;
        Bio = bio;
        AvatarUrl = avatarUrl;
        AuditTrail.Add(new AuditEntry {
            Action = "Profile Updated",
            UserId = Id,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    public void VerifyEmail() {
        IsEmailVerified = true;
        AuditTrail.Add(new AuditEntry {
            Action = "Email Verified",
            UserId = Id,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    public void EnablePublisher() {
        IsPublisher = true;
        AuditTrail.Add(new AuditEntry {
            Action = "Publisher Enabled",
            UserId = Id,
            DateTime = DateTimeOffset.UtcNow
        });
    }
}