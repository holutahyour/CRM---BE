namespace CRM.Services.Implementations;

public class VendorService : MSSQLBaseService<Vendor, Guid>, IVendorService
{
    private readonly IMSSQLRepository<Vendor, Guid> _repository;

    public VendorService(
    IMSSQLRepository<Vendor, Guid> baseRepository,
    IMSSQLRepository<AuditLog, long> auditLogRepository,
    IApplicationDbContext context,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor
    )
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
        _repository = baseRepository;
    }
}
