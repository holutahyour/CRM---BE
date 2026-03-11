using CRM.Base.Common.Domain.Common;
using CRM.Base.Common.Domain.Entities;
using CRM.Base.Common.Repositories;
using CRM.Base.Common.Repositories.Interfaces;
using CRM.Base.Common.Services.Implementation;
using CRM.Domain.DTOs.Core;
using Microsoft.AspNetCore.Http;

public class MenuService : MSSQLBaseService<Menu, Guid>, IMenuService
{
    private readonly IMSSQLRepository<Menu, Guid> _repository;
    private readonly IMSSQLRepository<User, Guid> _userRepository;
    private readonly IMSSQLRepository<Permission, Guid> _permissionRepository;
    private readonly IMSSQLRepository<TenantModule, Guid> _tenantModuleRepository;

    public MenuService(
    IMSSQLRepository<Menu, Guid> repository,
    IMSSQLRepository<User, Guid> userRepository,
    IMSSQLRepository<Permission, Guid> permissionRepository,
    IMSSQLRepository<TenantModule, Guid> tenantModuleRepository,
    IMSSQLRepository<AuditLog, long> auditLogRepository,
    IApplicationDbContext context,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor
    )
        : base(repository, auditLogRepository, context, mapper, httpContextAccessor)
    {
        _repository = repository;
        _userRepository = userRepository;
        _permissionRepository = permissionRepository;
        _tenantModuleRepository = tenantModuleRepository;
    }

    public virtual async Task<Result<IEnumerable<MenuDTO>>> GetMyMenusAsync(string oid, Guid tenantId)
    {
        Result<IEnumerable<MenuDTO>> result = new(false);

        try
        {
            // 1. Get user with roles
            var user = await _userRepository.GetAsync(u => u.EntraObjectId == oid && u.TenantId == tenantId);
            if (user == null) { result.SetError("User not found", ""); return result; }

            var isAdmin = user.UserRoles.Any(ur => ur.Role.Code == "ADMIN");

            // 2. Get user's permissions
            var userPermissions = isAdmin
                ? new HashSet<string>((await _permissionRepository.GetAllAsync()).Select(p => p.Code).ToList())
                : [.. user.UserRoles
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Select(rp => rp.Permission.Code)
                    .Distinct()];

            // 2. Get tenant's active modules
            var tenantModules = (await _tenantModuleRepository.GetAllAsync(tm => tm.TenantId == tenantId && tm.IsActive))
                .Select(tm => tm.Module.Code)
                .ToList();

            // 3. Filter menus
            var allMenus = (await _repository.GetAllAsync(m => m.IsActive))
                .Where(m => m.ModuleCode == null || tenantModules.Contains(m.ModuleCode))
                .OrderBy(m => m.Position)
                .ToList();

            var accessible = allMenus
                .Where(m => !m.MenuPermissions.Any() ||
                             m.MenuPermissions.Any(mp => userPermissions.Contains(mp.Permission.Code)))
                .ToList();

            // 4. Build tree
            var tree = BuildTree(accessible, null);

            result.SetSuccess(tree, "Retrieved Successfully.");

        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Error while retrieving Base");
        }

        return result;
    }

    private static List<MenuDTO> BuildTree(List<Menu> menus, Guid? parentId)
    {
        return menus
            .Where(m => m.ParentId == parentId)
            .Select(m => new MenuDTO(m.Id, m.Name, m.Label, m.Icon, m.Route, m.Position,
                BuildTree(menus, m.Id),
                m.MenuPermissions.Select(mp => mp.PermissionId).ToList()))
            .OrderBy(m => m.Position)
            .ToList();
    }
}