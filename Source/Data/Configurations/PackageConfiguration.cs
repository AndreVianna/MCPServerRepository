using MCPHub.Domain.Entities;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.Data.Configurations;

public class PackageConfiguration : IEntityTypeConfiguration<Package> {
    public void Configure(EntityTypeBuilder<Package> builder) {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(p => p.Version)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Repository)
            .HasMaxLength(500);

        builder.Property(p => p.License)
            .HasMaxLength(100);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.TrustTier)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.Tags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>(),
                new ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()))
            .HasColumnType("jsonb");

        // Indexes
        builder.HasIndex(p => p.Name)
            .IsUnique();

        builder.HasIndex(p => p.PublisherId);

        builder.HasIndex(p => p.Status);

        builder.HasIndex(p => p.TrustTier);

        // Full-text search index for PostgreSQL
        builder.HasIndex(p => new { p.Name, p.Description })
            .HasMethod("gin")
            .HasDatabaseName("IX_Packages_FullText");

        // Configure AuditTrail as JSON column
        builder.OwnsMany(p => p.AuditTrail, auditBuilder => {
            auditBuilder.ToJson();
            auditBuilder.Property(a => a.Action).IsRequired();
            auditBuilder.Property(a => a.UserId).IsRequired();
            auditBuilder.Property(a => a.DateTime).IsRequired();
        });

        // Relationships
        builder.HasOne(p => p.Publisher)
            .WithMany(pub => pub.Packages)
            .HasForeignKey(p => p.PublisherId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Versions)
            .WithOne(v => v.Package)
            .HasForeignKey(v => v.PackageId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure ScanResult as owned entity
        builder.OwnsOne(p => p.ScanResult, scan => {
            scan.Property(s => s.Status)
                .IsRequired()
                .HasConversion<string>();
            scan.Property(s => s.VulnerabilityCount)
                .IsRequired();
            scan.Property(s => s.HighestSeverity)
                .IsRequired()
                .HasConversion<string>();
            scan.Property(s => s.ScannedAt)
                .IsRequired();
            scan.Property(s => s.ScannerVersion)
                .IsRequired()
                .HasMaxLength(32);
            scan.Property(s => s.ScanLog)
                .HasMaxLength(4096);
            scan.Property(s => s.Vulnerabilities)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<List<SecurityVulnerability>>(v, JsonSerializerOptions.Default) ?? new List<SecurityVulnerability>())
                .HasColumnType("jsonb");
        });
    }
}