using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Data.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).HasMaxLength(150).IsRequired();
        builder.Property(t => t.Code).HasMaxLength(20);
        builder.Property(t => t.RegistrationNumber).HasMaxLength(50);
        builder.Property(t => t.IndustryType).HasMaxLength(50);
        builder.Property(t => t.Address).HasMaxLength(500);
        builder.Property(t => t.ContactEmail).HasMaxLength(150);
        builder.Property(t => t.ContactPhone).HasMaxLength(30);
        builder.Property(t => t.SubscriptionPlan).HasMaxLength(50);
        builder.Property(t => t.SubscriptionStatus)
            .HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.RowVersion).IsRowVersion();

        builder.HasIndex(t => t.Code).IsUnique()
            .HasFilter("[Code] IS NOT NULL");
    }
}
