namespace Data.Configurations;

public class ServerVersionConfiguration : IEntityTypeConfiguration<ServerVersion>
{
    public void Configure(EntityTypeBuilder<ServerVersion> builder)
    {
        builder.HasKey(v => v.Id);
        
        builder.Property(v => v.ServerId)
            .IsRequired();
        
        builder.Property(v => v.Version)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(v => v.ManifestJson)
            .IsRequired()
            .HasColumnType("jsonb");
        
        builder.Property(v => v.ReleaseNotes)
            .HasMaxLength(1000);
        
        builder.Property(v => v.PackagePath)
            .HasMaxLength(255);
        
        builder.Property(v => v.PackageHash)
            .HasMaxLength(64);
        
        builder.Property(v => v.TrustTier)
            .IsRequired()
            .HasConversion<string>();
        
        builder.Property(v => v.PublishedAt)
            .IsRequired();
        
        builder.Property(v => v.CreatedAt)
            .IsRequired();
        
        builder.Property(v => v.UpdatedAt)
            .IsRequired();
        
        // Indexes
        builder.HasIndex(v => new { v.ServerId, v.Version })
            .IsUnique();
        
        builder.HasIndex(v => v.ServerId);
        
        builder.HasIndex(v => v.TrustTier);
        
        builder.HasIndex(v => v.PublishedAt);
        
        builder.HasIndex(v => v.IsPrerelease);
        
        builder.HasIndex(v => v.IsDeprecated);
        
        // Relationships
        builder.HasOne(v => v.Server)
            .WithMany(s => s.Versions)
            .HasForeignKey(v => v.ServerId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(v => v.SecurityScans)
            .WithOne(s => s.Version)
            .HasForeignKey(s => s.VersionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}