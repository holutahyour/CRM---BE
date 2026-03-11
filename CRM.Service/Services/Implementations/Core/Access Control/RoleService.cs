using CRM.Base.Common.Domain.Entities;
using CRM.Base.Common.Repositories;
using CRM.Base.Common.Repositories.Interfaces;
using CRM.Base.Common.Services.Implementation;
using CRM.Base.Enums;
using CRM.Domain.DTOs.Core;
using CRM.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CRM.Base.Common.Domain.Common;

public class RoleService : MSSQLBaseService<Role, Guid>, IRoleService
{
    private readonly IMSSQLRepository<Role, Guid> _roleRepository;
    private readonly IMSSQLRepository<RolePermission, Guid> _rolePermissionRepository;
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;

    public RoleService(
        IMSSQLRepository<Role, Guid> baseRepository,
        IMSSQLRepository<RolePermission, Guid> rolePermissionRepository,
        IMSSQLRepository<AuditLog, long> auditLogRepository,
        IApplicationDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor
    )
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
        _roleRepository = baseRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _mapper = mapper;
        _context = context;
    }

    public override async Task<Result<TResponse>> CreateAsync<TResponse, TRequest>(TRequest request)
    {
        if (request is CreateRoleRequest createRequest)
        {
            var result = new Result<TResponse>(false);
            try
            {
                var role = _mapper.Map<Role>(createRequest);
                role.Code = createRequest.Code; // Use provided code

                var response = await _roleRepository.CreateAsync(role);

                if (createRequest.PermissionIds != null && createRequest.PermissionIds.Any())
                {
                    foreach (var permissionId in createRequest.PermissionIds)
                    {
                        await _rolePermissionRepository.CreateAsync(new RolePermission
                        {
                            RoleId = role.Id,
                            PermissionId = permissionId
                        });
                    }
                }

                await _context.SaveChangesAsync();
                result.SetSuccess(_mapper.Map<TResponse>(response), "Role created successfully with permissions.");
                return result;
            }
            catch (Exception ex)
            {
                result.SetError(ex.ToString(), "Error while creating Role");
                return result;
            }
        }

        return await base.CreateAsync<TResponse, TRequest>(request);
    }

    public override async Task<Result<bool>> UpdateAsync<TRequest>(Guid id, TRequest request)
    {
        if (request is UpdateRoleRequest updateRequest)
        {
            var result = new Result<bool>(false);
            try
            {
                var existingRole = await _roleRepository.GetByIdAsync(id);
                if (existingRole == null)
                {
                    result.SetError("Role not found", "Role not found");
                    return result;
                }

                _mapper.Map(updateRequest, existingRole);

                // Update permissions
                var existingPermissions = await _rolePermissionRepository.GetAllAsync(rp => rp.RoleId == id);
                foreach (var ep in existingPermissions)
                {
                    await _rolePermissionRepository.DeleteAsync(ep.Id);
                }

                if (updateRequest.PermissionIds != null && updateRequest.PermissionIds.Any())
                {
                    foreach (var permissionId in updateRequest.PermissionIds)
                    {
                        await _rolePermissionRepository.CreateAsync(new RolePermission
                        {
                            RoleId = id,
                            PermissionId = permissionId
                        });
                    }
                }

                await _context.SaveChangesAsync();
                result.SetSuccess(true, "Role updated successfully with permissions.");
                return result;
            }
            catch (Exception ex)
            {
                result.SetError(ex.ToString(), "Error while updating Role");
                return result;
            }
        }

        return await base.UpdateAsync(id, request);
    }
}