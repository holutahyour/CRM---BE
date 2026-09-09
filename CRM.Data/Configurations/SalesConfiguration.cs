using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Data.Configurations;

// Every Sales tab reads its records for one tenant in date order, so each table
// is indexed on (TenantId, Date).

public class SalesDailyProductionConfiguration : IEntityTypeConfiguration<SalesDailyProduction>
{
    public void Configure(EntityTypeBuilder<SalesDailyProduction> builder)
    {
        builder.ToTable("sales_daily_production");
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => new { p.TenantId, p.Date });
    }
}

public class SalesRecordConfiguration : IEntityTypeConfiguration<SalesRecord>
{
    public void Configure(EntityTypeBuilder<SalesRecord> builder)
    {
        builder.ToTable("sales_records");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Customer).HasMaxLength(200).IsRequired();
        builder.Property(s => s.ModeOfPayment).HasMaxLength(50).IsRequired();
        builder.Property(s => s.Remarks).HasMaxLength(1000);
        builder.Property(s => s.Quantity).HasPrecision(18, 4);
        builder.Property(s => s.Price).HasPrecision(18, 4);
        builder.Property(s => s.Paid).HasPrecision(18, 4);
        builder.HasIndex(s => new { s.TenantId, s.Date });
        builder.HasIndex(s => s.Customer);
    }
}

public class SalesFeedCostConfiguration : IEntityTypeConfiguration<SalesFeedCost>
{
    public void Configure(EntityTypeBuilder<SalesFeedCost> builder)
    {
        builder.ToTable("sales_feed_costs");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.FeedType).HasMaxLength(200).IsRequired();
        builder.Property(f => f.Quantity).HasPrecision(18, 4);
        builder.Property(f => f.CostPerBag).HasPrecision(18, 4);
        builder.HasIndex(f => new { f.TenantId, f.Date });
    }
}

public class SalesStockRecordConfiguration : IEntityTypeConfiguration<SalesStockRecord>
{
    public void Configure(EntityTypeBuilder<SalesStockRecord> builder)
    {
        builder.ToTable("sales_stock_records");
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => new { s.TenantId, s.Date });
    }
}
