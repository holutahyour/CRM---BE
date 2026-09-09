namespace CRM.Services.Interfaces;

// The four Sales datasets are plain per-tenant record logs, so each one takes
// the generic CRUD from MSSQLBaseService without additions.

public interface ISalesDailyProductionService : IMSSQLBaseService<SalesDailyProduction, Guid>
{
}

public interface ISalesRecordService : IMSSQLBaseService<SalesRecord, Guid>
{
}

public interface ISalesFeedCostService : IMSSQLBaseService<SalesFeedCost, Guid>
{
}

public interface ISalesStockRecordService : IMSSQLBaseService<SalesStockRecord, Guid>
{
}
