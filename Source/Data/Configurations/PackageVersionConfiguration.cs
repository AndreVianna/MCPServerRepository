namespace Data.Configurations;

public class PackageVersionConfiguration : IEntityTypeConfiguration<PackageVersion>
{
    public void Configure(EntityTypeBuilder<PackageVersion> builder)
    {
        builder.HasKey(pv => pv.Id);
        
        builder.Property(pv => pv.Version)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(pv => pv.ReleaseNotes)
            .HasMaxLength(5000);
        
        builder.Property(pv => pv.DownloadUrl)
            .IsRequired()
            .HasMaxLength(1000);
        
        builder.Property(pv => pv.FileSize)
            .IsRequired();
        
        builder.Property(pv => pv.FileHash)
            .IsRequired()
            .HasMaxLength(128);
        
        builder.Property(pv => pv.IsPrerelease)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.Property(pv => pv.CreatedAt)
            .IsRequired();
        
        builder.Property(pv => pv.Dependencies)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<PackageDependency>>(v, JsonSerializerOptions.Default) ?? new List<PackageDependency>(),
                new ValueComparer<List<PackageDependency>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()))
            .HasColumnType("jsonb");
        
        // Indexes
        builder.HasIndex(pv => pv.PackageId);
        
        builder.HasIndex(pv => pv.Version);
        
        builder.HasIndex(pv => pv.CreatedAt);
        
        builder.HasIndex(pv => new { pv.PackageId, pv.Version })
            .IsUnique();
        
        // Relationships
        builder.HasOne(pv => pv.Package)
            .WithMany(p => p.Versions)
            .HasForeignKey(pv => pv.PackageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}