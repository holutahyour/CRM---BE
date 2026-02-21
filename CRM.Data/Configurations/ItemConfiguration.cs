using CRM.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Data.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("items");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Sku).HasMaxLength(50).IsRequired();
        builder.Property(i => i.Name).HasMaxLength(200).IsRequired();
        builder.Property(i => i.UnitType).HasMaxLength(30).IsRequired();
        builder.Property(i => i.Barcode).HasMaxLength(100);
        builder.Property(i => i.ImageUrl).HasMaxLength(500);
        builder.Property(i => i.StorageConditions).HasMaxLength(500);
        builder.Property(i => i.MinStockLevel).HasPrecision(18, 4);
        builder.Property(i => i.ReorderQuantity).HasPrecision(18, 4);
        builder.Property(i => i.CostPrice).HasPrecision(18, 4);
        builder.Property(i => i.SellingPrice).HasPrecision(18, 4);
        builder.Property(i => i.RowVersion).IsRowVersion();

        builder.HasIndex(i => new { i.TenantId, i.Sku }).IsUnique();
        builder.HasIndex(i => i.Barcode);
        builder.HasIndex(i => i.Name);

        builder.HasOne(i => i.Category)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(i => i.Vendor)
            .WithMany()
            .HasForeignKey(i => i.VendorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}