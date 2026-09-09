namespace CRM.Services.Implementations;

public class SalesDailyProductionService
    : MSSQLBaseService<SalesDailyProduction, Guid>, ISalesDailyProductionService
{
    public SalesDailyProductionService(
        IMSSQLRepository<SalesDailyProduction, Guid> baseRepository,
        IMSSQLRepository<AuditLog, long> auditLogRepository,
        IApplicationDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
    }
}

public class SalesRecordService : MSSQLBaseService<SalesRecord, Guid>, ISalesRecordService
{
    public SalesRecordService(
        IMSSQLRepository<SalesRecord, Guid> baseRepository,
        IMSSQLRepository<AuditLog, long> auditLogRepository,
        IApplicationDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
    }
}

public class SalesFeedCostService : MSSQLBaseService<SalesFeedCost, Guid>, ISalesFeedCostService
{
    public SalesFeedCostService(
        IMSSQLRepository<SalesFeedCost, Guid> baseRepository,
        IMSSQLRepository<AuditLog, long> auditLogRepository,
        IApplicationDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
    }
}

public class SalesStockRecordService
    : MSSQLBaseService<SalesStockRecord, Guid>, ISalesStockRecordService
{
    public SalesStockRecordService(
        IMSSQLRepository<SalesStockRecord, Guid> baseRepository,
        IMSSQLRepository<AuditLog, long> auditLogRepository,
        IApplicationDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
    }
}
