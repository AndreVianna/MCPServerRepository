using System.Text.Json;
using MCPHub.Domain.Entities;
using MCPHub.Domain.ValueObjects;
using MCPHub.Domain.Common;

namespace MCPHub.Data.Configurations;

public class SecurityScanConfiguration : IEntityTypeConfiguration<SecurityScan> {
    public void Configure(EntityTypeBuilder<SecurityScan> builder) {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.VersionId)
            .IsRequired();

        builder.Property(s => s.ServerVersionId)
            .IsRequired();

        builder.Property(s => s.ScanType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>();


        builder.Property(s => s.ScanStartedAt)
            .IsRequired();

        builder.Property(s => s.ScanCompletedAt);

        builder.Property(s => s.CriticalIssues)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.ErrorMessage)
            .HasMaxLength(4096);

        builder.Property(s => s.ScannerVersion)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(s => s.Metadata)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, JsonSerializerOptions.Default) ?? new Dictionary<string, object>()
            );

        builder.Property(s => s.Result)
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => v == null ? null : JsonSerializer.Deserialize<SecurityScanResult>(v, JsonSerializerOptions.Default)
            );

        // Indexes
        builder.HasIndex(s => s.VersionId);

        builder.HasIndex(s => s.ServerVersionId);

        builder.HasIndex(s => s.ScanType);

        builder.HasIndex(s => s.Status);

        builder.HasIndex(s => s.ScanStartedAt);

        builder.HasIndex(s => s.ScanCompletedAt);

        builder.HasIndex(s => s.CriticalIssues);

        builder.HasIndex(s => new { s.VersionId, s.ScanType });

        // Configure AuditTrail as JSON column
        builder.OwnsMany(s => s.AuditTrail, auditBuilder => {
            auditBuilder.ToJson();
            auditBuilder.Property(a => a.Action).IsRequired();
            auditBuilder.Property(a => a.UserId).IsRequired();
            auditBuilder.Property(a => a.DateTime).IsRequired();
        });

        // Relationships
        builder.HasOne(s => s.ServerVersion)
            .WithMany(v => v.SecurityScans)
            .HasForeignKey(s => s.ServerVersionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}