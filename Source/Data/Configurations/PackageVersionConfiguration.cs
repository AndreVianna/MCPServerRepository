using System.Text.Json;
using MCPHub.Domain.Entities;
using MCPHub.Domain.ValueObjects;
using MCPHub.Domain.Common;

namespace MCPHub.Data.Configurations;

public class PackageVersionConfiguration : IEntityTypeConfiguration<PackageVersion> {
    public void Configure(EntityTypeBuilder<PackageVersion> builder) {
        builder.HasKey(pv => pv.Id);

        builder.Property(pv => pv.Version)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(pv => pv.ReleaseNotes)
            .HasMaxLength(4096);

        builder.Property(pv => pv.DownloadUrl)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(pv => pv.ChecksumSha256)
            .IsRequired()
            .HasMaxLength(4096);

        builder.Property(pv => pv.FileSize)
            .IsRequired();

        builder.Property(pv => pv.IsPrerelease)
            .IsRequired()
            .HasDefaultValue(false);


        // Indexes
        builder.HasIndex(pv => pv.PackageId);

        builder.HasIndex(pv => pv.Version);


        builder.HasIndex(pv => new { pv.PackageId, pv.Version })
            .IsUnique();

        // Configure AuditTrail as JSON column
        builder.OwnsMany(pv => pv.AuditTrail, auditBuilder => {
            auditBuilder.ToJson();
            auditBuilder.Property(a => a.Action).IsRequired();
            auditBuilder.Property(a => a.UserId).IsRequired();
            auditBuilder.Property(a => a.DateTime).IsRequired();
        });

        // Relationships
        builder.HasOne(pv => pv.Package)
            .WithMany(p => p.Versions)
            .HasForeignKey(pv => pv.PackageId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure ScanResult as owned entity
        builder.OwnsOne(pv => pv.ScanResult, scan => {
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