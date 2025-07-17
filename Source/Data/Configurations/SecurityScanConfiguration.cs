namespace Data.Configurations;

public class SecurityScanConfiguration : IEntityTypeConfiguration<SecurityScan>
{
    public void Configure(EntityTypeBuilder<SecurityScan> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.VersionId)
            .IsRequired();
        
        builder.Property(s => s.ScanType)
            .IsRequired()
            .HasConversion<string>();
        
        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>();
        
        builder.Property(s => s.StartedAt)
            .IsRequired();
        
        builder.Property(s => s.Score)
            .IsRequired()
            .HasDefaultValue(0);
        
        builder.Property(s => s.ResultsJson)
            .HasColumnType("jsonb");
        
        builder.Property(s => s.ErrorMessage)
            .HasMaxLength(1000);
        
        builder.Property(s => s.CriticalIssues)
            .IsRequired()
            .HasDefaultValue(0);
        
        builder.Property(s => s.HighIssues)
            .IsRequired()
            .HasDefaultValue(0);
        
        builder.Property(s => s.MediumIssues)
            .IsRequired()
            .HasDefaultValue(0);
        
        builder.Property(s => s.LowIssues)
            .IsRequired()
            .HasDefaultValue(0);
        
        builder.Property(s => s.CreatedAt)
            .IsRequired();
        
        builder.Property(s => s.UpdatedAt)
            .IsRequired();
        
        // Indexes
        builder.HasIndex(s => s.VersionId);
        
        builder.HasIndex(s => s.ScanType);
        
        builder.HasIndex(s => s.Status);
        
        builder.HasIndex(s => s.StartedAt);
        
        builder.HasIndex(s => s.CompletedAt);
        
        builder.HasIndex(s => s.CriticalIssues);
        
        builder.HasIndex(s => new { s.VersionId, s.ScanType });
        
        // Relationships
        builder.HasOne(s => s.Version)
            .WithMany(v => v.SecurityScans)
            .HasForeignKey(s => s.VersionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}