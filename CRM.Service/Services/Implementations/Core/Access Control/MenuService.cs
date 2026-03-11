using CRM.Base.Common.Domain.Common;
using CRM.Base.Common.Domain.Entities;
using CRM.Base.Common.Repositories;
using CRM.Base.Common.Repositories.Interfaces;
using CRM.Base.Common.Services.Implementation;
using Microsoft.AspNetCore.Http;

public class MenuService : MSSQLBaseService<Menu, Guid>, IMenuService
{
    private readonly IMSSQLRepository<MenuPermission, Guid> _menuPermissionRepository;
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;
    private readonly IMSSQLRepository<Menu, Guid> _repository;
    private readonly IMSSQLRepository<User, Guid> _userRepository;
    private readonly IMSSQLRepository<Permission, Guid> _permissionRepository;
    private readonly IMSSQLRepository<TenantModule, Guid> _tenantModuleRepository;

    public MenuService(
    IMSSQLRepository<Menu, Guid> repository,
    IMSSQLRepository<User, Guid> userRepository,
    IMSSQLRepository<Permission, Guid> permissionRepository,
    IMSSQLRepository<TenantModule, Guid> tenantModuleRepository,
    IMSSQLRepository<MenuPermission, Guid> menuPermissionRepository,
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
        _menuPermissionRepository = menuPermissionRepository;
        _mapper = mapper;
        _context = context;
    }

    public override async Task<Result<TResponse>> CreateAsync<TResponse, TRequest>(TRequest request)
    {
        if (request is CreateMenuRequest createRequest)
        {
            var result = new Result<TResponse>(false);
            try
            {
                var menu = _mapper.Map<Menu>(createRequest);
                var response = await _repository.CreateAsync(menu);

                if (createRequest.PermissionIds != null && createRequest.PermissionIds.Any())
                {
                    foreach (var permissionId in createRequest.PermissionIds)
                    {
                        await _menuPermissionRepository.CreateAsync(new MenuPermission
                        {
                            MenuId = menu.Id,
                            PermissionId = permissionId
                        });
                    }
                }

                await _context.SaveChangesAsync();
                result.SetSuccess(_mapper.Map<TResponse>(response), "Menu created successfully with permissions.");
                return result;
            }
            catch (Exception ex)
            {
                result.SetError(ex.ToString(), "Error while creating Menu");
                return result;
            }
        }

        return await base.CreateAsync<TResponse, TRequest>(request);
    }

    public override async Task<Result<bool>> UpdateAsync<TRequest>(Guid id, TRequest request)
    {
        if (request is UpdateMenuRequest updateRequest)
        {
            var result = new Result<bool>(false);
            try
            {
                var existingMenu = await _repository.GetByIdAsync(id);
                if (existingMenu == null)
                {
                    result.SetError("Menu not found", "Menu not found");
                    return result;
                }

                _mapper.Map(updateRequest, existingMenu);

                // Update permissions
                var existingPermissions = await _menuPermissionRepository.GetAllAsync(mp => mp.MenuId == id);
                foreach (var ep in existingPermissions)
                {
                    await _menuPermissionRepository.DeleteAsync(ep.Id);
                }

                if (updateRequest.PermissionIds != null && updateRequest.PermissionIds.Any())
                {
                    foreach (var permissionId in updateRequest.PermissionIds)
                    {
                        await _menuPermissionRepository.CreateAsync(new MenuPermission
                        {
                            MenuId = id,
                            PermissionId = permissionId
                        });
                    }
                }

                await _context.SaveChangesAsync();
                result.SetSuccess(true, "Menu updated successfully with permissions.");
                return result;
            }
            catch (Exception ex)
            {
                result.SetError(ex.ToString(), "Error while updating Menu");
                return result;
            }
        }

        return await base.UpdateAsync(id, request);
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