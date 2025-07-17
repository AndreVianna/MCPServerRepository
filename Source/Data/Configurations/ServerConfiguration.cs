namespace Data.Configurations;

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

        builder.Property(s => s.RepositoryUrl)
            .HasMaxLength(255);

        builder.Property(s => s.DocumentationUrl)
            .HasMaxLength(255);

        builder.Property(s => s.LicenseUrl)
            .HasMaxLength(255);

        builder.Property(s => s.License)
            .HasMaxLength(100);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(s => s.Name)
            .IsUnique();

        builder.HasIndex(s => s.PublisherId);

        builder.HasIndex(s => s.CreatedAt);

        builder.HasIndex(s => s.IsDeprecated);

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