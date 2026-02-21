using CRM.Base.Common.Domain.Common;
using CRM.Base.Common.Domain.Entities;
using CRM.Base.Common.Repositories;
using CRM.Base.Common.Repositories.Interfaces;
using CRM.Base.Common.Services.Implementation;
using CRM.Domain.Enums;
using Microsoft.AspNetCore.Http;

public class TenantService : MSSQLBaseService<Tenant, Guid>, ITenantService
{
    private readonly IMSSQLRepository<Tenant, Guid> _repository;
    private readonly IMSSQLRepository<Role, Guid> _roleRepository;
    private readonly IMSSQLRepository<Permission, Guid> _permissionRepository;
    private readonly IMSSQLRepository<RolePermission, Guid> _rolePermissionRepository;
    private readonly IMSSQLRepository<Module, Guid> _moduleRepository;
    private readonly IMSSQLRepository<TenantModule, Guid> _tenantModuleRepository;
    private readonly IApplicationDbContext _context;

    public TenantService(
    IMSSQLRepository<Tenant, Guid> repository,
    IMSSQLRepository<Role, Guid> roleRepository,
    IMSSQLRepository<Permission, Guid> permissionRepository,
    IMSSQLRepository<RolePermission, Guid> rolePermissionRepository,
    IMSSQLRepository<Module, Guid> moduleRepository,
    IMSSQLRepository<TenantModule, Guid> tenantModuleRepository,
    IMSSQLRepository<AuditLog, long> auditLogRepository,
    IApplicationDbContext context,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor
    )
        : base(repository, auditLogRepository, context, mapper, httpContextAccessor)
    {
        _repository = repository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _moduleRepository = moduleRepository;
        _tenantModuleRepository = tenantModuleRepository;
        _context = context;
    }
    public async Task<Result<Tenant>> OnboardAsync(string name, string? code, string adminEmail)
    {
        var result = new Result<Tenant>(false);

        try
        {
            var tenant = new Tenant
            {
                Name = name,
                Code = code,
                SubscriptionStatus = SubscriptionStatus.Trial,
                TrialEndDate = DateTime.UtcNow.AddDays(30)
            };
            await _repository.CreateAsync(tenant);

            await _context.SaveChangesAsync();

            // Seed default roles
            var adminRole = new Role { TenantId = tenant.Id, Name = "Administrator", Code = "ADMIN", IsSystem = true };
            var managerRole = new Role { TenantId = tenant.Id, Name = "Manager", Code = "MANAGER", IsSystem = true };
            var staffRole = new Role { TenantId = tenant.Id, Name = "Staff", Code = "STAFF", IsSystem = true };

            await _roleRepository.AddEntitiesAsync([adminRole, managerRole, staffRole]);

            // Assign all permissions to admin role
            var allPermissions = await _permissionRepository.GetAllAsync();
            foreach (var perm in allPermissions)
                await _rolePermissionRepository.CreateAsync(new RolePermission { TenantId = tenant.Id, RoleId = adminRole.Id, PermissionId = perm.Id });

            // Activate default modules
            var defaultModules = await _moduleRepository.GetAllAsync(m => m.Code == "CORE" || m.Code == "INVENTORY");
            foreach (var mod in defaultModules)
                await _tenantModuleRepository.CreateAsync(new TenantModule { TenantId = tenant.Id, ModuleId = mod.Id, IsActive = true, ActivatedAt = DateTime.UtcNow });

            await _context.SaveChangesAsync();

            result.SetSuccess(tenant, $"Tenant onboarded successfully!");

        }
        catch (Exception ex)
        {

            result.SetError(ex.ToString(), $"Error while onboarding tenant");
        }

        return result;
    }

}