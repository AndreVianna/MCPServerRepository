using System.Text.Json;

using MCPHub.Domain.Common;
using MCPHub.Domain.Entities;

namespace MCPHub.Data.Configurations;

public class ServerConfiguration : IEntityTypeConfiguration<Server> {
    public void Configure(EntityTypeBuilder<Server> builder) {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.PublisherId)
            .IsRequired();

        builder.Property(s => s.Repository)
            .HasMaxLength(255);

        builder.Property(s => s.License)
            .HasMaxLength(100);

        builder.Property(s => s.Status)
            .IsRequired();

        builder.Property(s => s.TrustTier)
            .IsRequired();

        builder.Property(s => s.Tags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>()
            );

        // Indexes
        builder.HasIndex(s => s.Name)
            .IsUnique();

        builder.HasIndex(s => s.PublisherId);

        builder.HasIndex(s => s.Status);

        builder.HasIndex(s => s.TrustTier);

        // Configure AuditTrail as JSON column
        builder.OwnsMany(s => s.AuditTrail, auditBuilder => {
            auditBuilder.ToJson();
            auditBuilder.Property(a => a.Action).IsRequired();
            auditBuilder.Property(a => a.UserId).IsRequired();
            auditBuilder.Property(a => a.DateTime).IsRequired();
        });

        // Relationships
        builder.HasOne(s => s.Publisher)
            .WithMany(p => p.Servers)
            .HasForeignKey(s => s.PublisherId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Versions)
            .WithOne(v => v.Server)
            .HasForeignKey(v => v.ServerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}