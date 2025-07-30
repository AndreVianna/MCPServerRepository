using MCPHub.Domain.Entities;

namespace MCPHub.Data.Configurations;

/// <summary>
/// Entity Framework configuration for PackageInstallation entity
/// </summary>
public class PackageInstallationConfiguration : IEntityTypeConfiguration<PackageInstallation> {
    public void Configure(EntityTypeBuilder<PackageInstallation> builder) {
        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.Version)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pi => pi.UserId)
            .IsRequired(); // Required for installations

        builder.Property(pi => pi.InstallationPath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(pi => pi.InstalledAt)
            .IsRequired();

        builder.Property(pi => pi.UninstalledAt)
            .IsRequired(false); // Nullable

        builder.Property(pi => pi.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(pi => pi.ErrorMessage)
            .HasMaxLength(2000)
            .IsRequired(false); // Nullable

        builder.Property(pi => pi.ClientVersion)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pi => pi.InstallationOptions)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, JsonSerializerOptions.Default) ?? new Dictionary<string, object>())
            .HasColumnType("jsonb");

        // Indexes for performance
        builder.HasIndex(pi => pi.PackageId)
            .HasDatabaseName("IX_PackageInstallations_PackageId");

        builder.HasIndex(pi => pi.UserId)
            .HasDatabaseName("IX_PackageInstallations_UserId");

        builder.HasIndex(pi => new { pi.UserId, pi.PackageId })
            .HasDatabaseName("IX_PackageInstallations_UserPackage");

        builder.HasIndex(pi => pi.Status)
            .HasDatabaseName("IX_PackageInstallations_Status");

        builder.HasIndex(pi => pi.InstalledAt)
            .HasDatabaseName("IX_PackageInstallations_InstalledAt");

        builder.HasIndex(pi => pi.UninstalledAt)
            .HasDatabaseName("IX_PackageInstallations_UninstalledAt");

        builder.HasIndex(pi => new { pi.PackageId, pi.Version })
            .HasDatabaseName("IX_PackageInstallations_PackageVersion");

        // Index for finding stuck installations
        builder.HasIndex(pi => new { pi.Status, pi.InstalledAt })
            .HasDatabaseName("IX_PackageInstallations_StatusTime");

        // Configure AuditTrail as JSON column
        builder.OwnsMany(pi => pi.AuditTrail, auditBuilder => {
            auditBuilder.ToJson();
            auditBuilder.Property(a => a.Action).IsRequired();
            auditBuilder.Property(a => a.UserId).IsRequired();
            auditBuilder.Property(a => a.DateTime).IsRequired();
        });

        // Relationships
        builder.HasOne(pi => pi.Package)
            .WithMany() // No navigation property on Package side
            .HasForeignKey(pi => pi.PackageId)
            .OnDelete(DeleteBehavior.Cascade);

        // Table configuration
        builder.ToTable("PackageInstallations");
    }
}