using MCPHub.Domain.Entities;
using MCPHub.Domain.Common;

namespace MCPHub.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser> {
    public void Configure(EntityTypeBuilder<ApplicationUser> builder) {
        // Configure custom properties (IdentityUser properties are configured by Identity)
        builder.Property(u => u.DisplayName)
            .HasMaxLength(100);

        builder.Property(u => u.GitHubUsername)
            .HasMaxLength(39); // GitHub username max length

        builder.Property(u => u.TwitterHandle)
            .HasMaxLength(15); // Twitter handle max length

        builder.Property(u => u.Website)
            .HasMaxLength(500);

        builder.Property(u => u.Bio)
            .HasMaxLength(500);

        builder.Property(u => u.AvatarUrl)
            .HasMaxLength(500);


        builder.Property(u => u.IsEmailVerified)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.IsPublisher)
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes for performance
        builder.HasIndex(u => u.GitHubUsername)
            .IsUnique()
            .HasFilter($"[{nameof(ApplicationUser.GitHubUsername)}] IS NOT NULL");

        builder.HasIndex(u => u.TwitterHandle)
            .IsUnique()
            .HasFilter($"[{nameof(ApplicationUser.TwitterHandle)}] IS NOT NULL");

        builder.HasIndex(u => u.IsPublisher);


        // Configure AuditTrail as JSON column
        builder.OwnsMany(u => u.AuditTrail, auditBuilder => {
            auditBuilder.ToJson();
            auditBuilder.Property(a => a.Action).IsRequired();
            auditBuilder.Property(a => a.UserId).IsRequired();
            auditBuilder.Property(a => a.DateTime).IsRequired();
        });

        // Relationships
        builder.HasMany(u => u.Publishers)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}