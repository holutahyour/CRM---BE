public class ModuleService : MSSQLBaseService<Module, Guid>, IModuleService
{
    private readonly IMSSQLRepository<Module, Guid> _moduleRepository;
    private readonly IMSSQLRepository<TenantModule, Guid> _tenantModuleRepository;
    private readonly IApplicationDbContext _context;

    public ModuleService(
        IMSSQLRepository<Module, Guid> baseRepository,
        IMSSQLRepository<TenantModule, Guid> tenantModuleRepository,
        IMSSQLRepository<AuditLog, long> auditLogRepository,
        IApplicationDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor
    )
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
        _moduleRepository = baseRepository;
        _tenantModuleRepository = tenantModuleRepository;
        _context = context;
    }

    public async Task<Result<IList<ModuleDTO>>> GetCatalogAsync()
    {
        var result = new Result<IList<ModuleDTO>>(false);
        try
        {
            var modules = await _moduleRepository.GetAllAsync(m => true, "Category");
            var dtos = modules
                .OrderBy(m => m.Name)
                .Select(m => new ModuleDTO(
                    m.Id, m.Name, m.Code, m.Description, m.Version,
                    m.CategoryId, m.Category?.Name, m.IsActive))
                .ToList();
            result.SetSuccess(dtos, "Retrieved successfully.");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Error while retrieving module catalog.");
        }
        return result;
    }

    public async Task<Result<IList<TenantModuleDTO>>> GetTenantModulesAsync()
    {
        var result = new Result<IList<TenantModuleDTO>>(false);
        try
        {
            // TenantModule is tenant-scoped: the global query filter restricts to the current tenant.
            var tenantModules = await _tenantModuleRepository.GetAllAsync(tm => true, "Module", "Module.Category");
            var dtos = tenantModules
                .Select(tm => new TenantModuleDTO(
                    tm.Id, tm.ModuleId,
                    tm.Module?.Name ?? "", tm.Module?.Code ?? "",
                    tm.Module?.Category?.Name,
                    tm.IsActive, tm.ActivatedAt, tm.ExpiresAt))
                .ToList();
            result.SetSuccess(dtos, "Retrieved successfully.");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Error while retrieving tenant modules.");
        }
        return result;
    }

    public async Task<Result<bool>> ToggleAsync(ToggleModuleRequest request)
    {
        var result = new Result<bool>(false);
        try
        {
            var existing = await _tenantModuleRepository.GetAsync(tm => tm.ModuleId == request.ModuleId);

            if (existing == null)
            {
                if (request.Enabled)
                {
                    await _tenantModuleRepository.CreateAsync(new TenantModule
                    {
                        ModuleId = request.ModuleId,
                        IsActive = true,
                        ActivatedAt = DateTime.UtcNow
                    });
                }
                // disabling a module that was never enabled is a no-op
            }
            else
            {
                existing.IsActive = request.Enabled;
                if (request.Enabled && existing.ActivatedAt == null)
                    existing.ActivatedAt = DateTime.UtcNow;
                await _tenantModuleRepository.UpdateAsync(existing.Id, existing);
            }

            await _context.SaveChangesAsync();
            result.SetSuccess(true, request.Enabled ? "Module enabled." : "Module disabled.");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Error while toggling module.");
        }
        return result;
    }
}
