using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Data.Configurations;

// Operations → Processing (Batch Production Scheduling workbook). Each log is read for one tenant
// in date order, so the dated tables are indexed on (TenantId, date). Free-text columns are sized
// for the multi-line cells the workbook holds (a product list can run to a dozen lines).

public class ProcessingProductConfiguration : IEntityTypeConfiguration<ProcessingProduct>
{
    public void Configure(EntityTypeBuilder<ProcessingProduct> builder)
    {
        builder.ToTable("ops_products");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Property(p => p.ProductCode).HasMaxLength(100);
        builder.Property(p => p.Upc).HasMaxLength(50);
        builder.Property(p => p.Sku).HasMaxLength(50);
        builder.Property(p => p.RawMaterialId).HasMaxLength(50);
        builder.Property(p => p.ProcessingDuration).HasMaxLength(100);

        // Not unique: soft-deleted rows keep their name. The service rejects live duplicates.
        builder.HasIndex(p => new { p.TenantId, p.Name });
    }
}

public class OrderRequestConfiguration : IEntityTypeConfiguration<OrderRequest>
{
    public void Configure(EntityTypeBuilder<OrderRequest> builder)
    {
        builder.ToTable("ops_order_requests");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.CustomerCode).HasMaxLength(50);
        builder.Property(o => o.CustomerName).HasMaxLength(200).IsRequired();
        builder.Property(o => o.Products).HasMaxLength(2000).IsRequired();
        builder.Property(o => o.ActivitiesRequired).HasMaxLength(2000);
        builder.Property(o => o.VolumeRequired).HasMaxLength(2000);
        builder.Property(o => o.DeliveryLocation).HasMaxLength(500);
        builder.Property(o => o.ProductBatchNumber).HasMaxLength(100);
        builder.Property(o => o.Duration).HasMaxLength(100);
        builder.HasIndex(o => new { o.TenantId, o.RequestDate });
    }
}

public class ProductionBatchConfiguration : IEntityTypeConfiguration<ProductionBatch>
{
    public void Configure(EntityTypeBuilder<ProductionBatch> builder)
    {
        builder.ToTable("ops_production_batches");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.RawMaterialBatchId).HasMaxLength(100);
        builder.Property(b => b.CustomerCode).HasMaxLength(50);
        builder.Property(b => b.CustomerName).HasMaxLength(200);
        builder.Property(b => b.BatchCode).HasMaxLength(200);
        builder.Property(b => b.ProductNames).HasMaxLength(2000).IsRequired();
        builder.Property(b => b.ProductCode).HasMaxLength(500);
        builder.Property(b => b.Quantity).HasPrecision(18, 4);
        builder.Property(b => b.QuantityUnit).HasMaxLength(30);
        builder.Property(b => b.QuantityNotes).HasMaxLength(2000);
        builder.Property(b => b.LeadTime).HasMaxLength(100);
        builder.Property(b => b.WorkCenters).HasMaxLength(500);
        builder.Property(b => b.Operators).HasMaxLength(500);
        builder.Property(b => b.TaskDescription).HasMaxLength(2000);
        builder.Property(b => b.QualityChecks).HasMaxLength(500);
        builder.Property(b => b.QuantitySent).HasMaxLength(2000);
        builder.Property(b => b.LogisticsPersonnel).HasMaxLength(200);
        builder.Property(b => b.DeliveryStatus).HasMaxLength(100);
        builder.Property(b => b.OnTimeDeliveryPercent).HasPrecision(5, 2);
        builder.HasIndex(b => new { b.TenantId, b.StartDate });

        builder.HasOne(b => b.OrderRequest)
            .WithMany()
            .HasForeignKey(b => b.OrderRequestId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(b => b.Product)
            .WithMany()
            .HasForeignKey(b => b.ProductId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class YieldEntryConfiguration : IEntityTypeConfiguration<YieldEntry>
{
    public void Configure(EntityTypeBuilder<YieldEntry> builder)
    {
        builder.ToTable("ops_yield_entries");
        builder.HasKey(y => y.Id);
        builder.Property(y => y.Shift).HasMaxLength(20);
        builder.Property(y => y.InputUnit).HasMaxLength(30);
        builder.Property(y => y.InputQuantity).HasPrecision(18, 4);
        builder.Property(y => y.InputWeightKg).HasPrecision(18, 4);
        builder.Property(y => y.CutWeightKg).HasPrecision(18, 4);
        builder.Property(y => y.DehydratedWeightKg).HasPrecision(18, 4);
        builder.Property(y => y.GrindWeightKg).HasPrecision(18, 4);
        builder.Property(y => y.SecondGrindWeightKg).HasPrecision(18, 4);
        builder.Property(y => y.WasteKg).HasPrecision(18, 4);
        builder.Property(y => y.Notes).HasMaxLength(1000);
        builder.HasIndex(y => new { y.TenantId, y.Date });
        builder.HasIndex(y => y.ProduceItemId);

        // Restrict: a produce with recorded runs must not vanish from under its yield history.
        builder.HasOne(y => y.ProduceItem)
            .WithMany()
            .HasForeignKey(y => y.ProduceItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(y => y.ProductionBatch)
            .WithMany()
            .HasForeignKey(y => y.ProductionBatchId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
