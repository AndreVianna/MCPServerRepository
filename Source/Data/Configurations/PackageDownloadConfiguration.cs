using System.Text.Json;

using MCPHub.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MCPHub.Data.Configurations;

/// <summary>
/// Entity Framework configuration for PackageDownload entity
/// </summary>
public class PackageDownloadConfiguration : IEntityTypeConfiguration<PackageDownload> {
    public void Configure(EntityTypeBuilder<PackageDownload> builder) {
        builder.HasKey(pd => pd.Id);

        builder.Property(pd => pd.Version)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pd => pd.UserId)
            .IsRequired(false); // Nullable for anonymous downloads

        builder.Property(pd => pd.IpAddress)
            .IsRequired()
            .HasMaxLength(45); // IPv6 max length

        builder.Property(pd => pd.UserAgent)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(pd => pd.DownloadedAt)
            .IsRequired();

        builder.Property(pd => pd.DownloadMethod)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(pd => pd.Metadata)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, JsonSerializerOptions.Default) ?? new Dictionary<string, object>())
            .HasColumnType("jsonb");

        // Indexes for performance
        builder.HasIndex(pd => pd.PackageId)
            .HasDatabaseName("IX_PackageDownloads_PackageId");

        builder.HasIndex(pd => new { pd.PackageId, pd.Version })
            .HasDatabaseName("IX_PackageDownloads_PackageVersion");

        builder.HasIndex(pd => pd.UserId)
            .HasDatabaseName("IX_PackageDownloads_UserId");

        builder.HasIndex(pd => pd.DownloadedAt)
            .HasDatabaseName("IX_PackageDownloads_DownloadedAt");

        builder.HasIndex(pd => new { pd.IpAddress, pd.PackageId, pd.DownloadedAt })
            .HasDatabaseName("IX_PackageDownloads_IpRateLimit");

        builder.HasIndex(pd => pd.DownloadMethod)
            .HasDatabaseName("IX_PackageDownloads_DownloadMethod");

        // Configure AuditTrail as JSON column
        builder.OwnsMany(pd => pd.AuditTrail, auditBuilder => {
            auditBuilder.ToJson();
            auditBuilder.Property(a => a.Action).IsRequired();
            auditBuilder.Property(a => a.UserId).IsRequired();
            auditBuilder.Property(a => a.DateTime).IsRequired();
        });

        // Relationships
        builder.HasOne(pd => pd.Package)
            .WithMany() // No navigation property on Package side
            .HasForeignKey(pd => pd.PackageId)
            .OnDelete(DeleteBehavior.Cascade);

        // Table configuration
        builder.ToTable("PackageDownloads");
    }
}