using MCPHub.Domain.Entities;

namespace MCPHub.Data.Configurations;

public class ServerVersionConfiguration : IEntityTypeConfiguration<ServerVersion> {
    public void Configure(EntityTypeBuilder<ServerVersion> builder) {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.ServerId)
            .IsRequired();

        builder.Property(v => v.Version)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(v => v.ReleaseNotes)
            .HasMaxLength(4096);

        builder.Property(v => v.PackageUrl)
            .HasMaxLength(256);

        builder.Property(v => v.PackageSize);

        builder.Property(v => v.Checksum)
            .HasMaxLength(64);

        builder.Property(v => v.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(v => v.CreatedAt)
            .IsRequired();

        builder.Property(v => v.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(v => new { v.ServerId, v.Version })
            .IsUnique();

        builder.HasIndex(v => v.ServerId);

        builder.HasIndex(v => v.Status);

        builder.HasIndex(v => v.CreatedAt);

        // Relationships
        builder.HasOne(v => v.Server)
            .WithMany(s => s.Versions)
            .HasForeignKey(v => v.ServerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.SecurityScans)
            .WithOne(s => s.ServerVersion)
            .HasForeignKey(s => s.ServerVersionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}