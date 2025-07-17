namespace Data.Configurations;

public class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
{
    public void Configure(EntityTypeBuilder<Publisher> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(p => p.Type)
            .IsRequired()
            .HasConversion<string>();
        
        builder.Property(p => p.CreatedAt)
            .IsRequired();
        
        builder.Property(p => p.UpdatedAt)
            .IsRequired();
        
        // Indexes
        builder.HasIndex(p => p.Name)
            .IsUnique();
        
        builder.HasIndex(p => p.Type);
        
        builder.HasIndex(p => p.CreatedAt);
        
        // Relationships
        builder.HasMany(p => p.Servers)
            .WithOne(s => s.Publisher)
            .HasForeignKey(s => s.PublisherId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(p => p.Packages)
            .WithOne(pkg => pkg.Publisher)
            .HasForeignKey(pkg => pkg.PublisherId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}