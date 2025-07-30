using MCPHub.Domain.Entities;

namespace MCPHub.Data.Configurations;

public class PublisherConfiguration : IEntityTypeConfiguration<Publisher> {
    public void Configure(EntityTypeBuilder<Publisher> builder) {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Type)
            .IsRequired()
            .HasConversion<string>();

        // Indexes
        builder.HasIndex(p => p.Name)
            .IsUnique();

        builder.HasIndex(p => p.Type);

        // Configure AuditTrail as JSON column
        builder.OwnsMany(p => p.AuditTrail, auditBuilder => {
            auditBuilder.ToJson();
            auditBuilder.Property(a => a.Action).IsRequired();
            auditBuilder.Property(a => a.UserId).IsRequired();
            auditBuilder.Property(a => a.DateTime).IsRequired();
        });

        // Relationships
        builder.HasOne(p => p.User)
            .WithMany(u => u.Publishers)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.SetNull);

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