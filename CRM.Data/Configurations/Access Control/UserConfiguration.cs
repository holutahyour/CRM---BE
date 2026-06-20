using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.EntraObjectId).HasMaxLength(100).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(150).IsRequired();
        builder.Property(u => u.FirstName).HasMaxLength(100);
        builder.Property(u => u.LastName).HasMaxLength(100);
        builder.Property(u => u.Phone).HasMaxLength(30);
        builder.Property(u => u.AvatarUrl).HasMaxLength(500);
        builder.Property(u => u.Position).HasMaxLength(150);
        //builder.Property(u => u.RowVersion).IsRowVersion();

        builder.HasIndex(u => u.EntraObjectId).IsUnique();
        builder.HasIndex(u => new { u.TenantId, u.Email }).IsUnique();

        // Self-referencing reporting line (downliner -> manager). Optional, no cascade so
        // deactivating a manager never deletes their downliners.
        builder.HasOne(u => u.Manager)
            .WithMany()
            .HasForeignKey(u => u.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}