using System.Text.Json;

using MCPHub.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MCPHub.Data.Configurations;

/// <summary>
/// Entity Framework configuration for TrustTierHistoryEntry
/// </summary>
public class TrustTierHistoryConfiguration : IEntityTypeConfiguration<TrustTierHistoryEntry> {
    public void Configure(EntityTypeBuilder<TrustTierHistoryEntry> builder) {
        builder.ToTable("TrustTierHistory");

        // Primary key
        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(t => t.PackageId)
            .IsRequired();

        builder.Property(t => t.FromTier)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.ToTier)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.Reason)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(t => t.ChangedAt)
            .IsRequired();

        builder.Property(t => t.ChangedByUserId)
            .IsRequired(false);

        builder.Property(t => t.ChangeType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.IsAutomatic)
            .IsRequired();

        builder.Property(t => t.TrustScoreAtChange)
            .IsRequired(false);

        builder.Property(t => t.AssessmentId)
            .IsRequired(false);

        // JSON column for metadata
        builder.Property(t => t.Metadata)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>())
            .HasColumnType("jsonb");

        // JSON column for audit trail
        builder.Property(t => t.AuditTrail)
            .HasConversion(
                v => JsonSerializer.Serialize(v.Cast<MCPHub.Domain.Common.AuditEntry>().ToList(), (JsonSerializerOptions?)null),
                v => (ICollection<MCPHub.Domain.Common.IAuditEntry>)(JsonSerializer.Deserialize<List<MCPHub.Domain.Common.AuditEntry>>(v, (JsonSerializerOptions?)null) ?? new List<MCPHub.Domain.Common.AuditEntry>()).Cast<MCPHub.Domain.Common.IAuditEntry>().ToList())
            .HasColumnType("jsonb");

        // Relationships
        builder.HasOne(t => t.Package)
            .WithMany()
            .HasForeignKey(t => t.PackageId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(t => t.PackageId)
            .HasDatabaseName("IX_TrustTierHistory_PackageId");

        builder.HasIndex(t => t.ChangedAt)
            .HasDatabaseName("IX_TrustTierHistory_ChangedAt");

        builder.HasIndex(t => new { t.FromTier, t.ToTier })
            .HasDatabaseName("IX_TrustTierHistory_TierTransition");

        builder.HasIndex(t => t.ChangeType)
            .HasDatabaseName("IX_TrustTierHistory_ChangeType");

        builder.HasIndex(t => t.ChangedByUserId)
            .HasDatabaseName("IX_TrustTierHistory_ChangedByUserId")
            .HasFilter("ChangedByUserId IS NOT NULL");

        builder.HasIndex(t => new { t.PackageId, t.ChangedAt })
            .HasDatabaseName("IX_TrustTierHistory_Package_ChangedAt");
    }
}