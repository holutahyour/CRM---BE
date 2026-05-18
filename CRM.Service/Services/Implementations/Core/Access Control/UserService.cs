using CRM.Base.Common;
using CRM.Base.Common.Domain.Common;
using CRM.Base.Common.Domain.Entities;
using CRM.Base.Common.Repositories;
using CRM.Base.Common.Repositories.Interfaces;
using CRM.Base.Common.Services.Implementation;
using CRM.Base.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

public class UserService : MSSQLBaseService<User, Guid>, IUserService
{
    private readonly IMSSQLRepository<User, Guid> _repository;
    private readonly IMSSQLRepository<UserRole, Guid> _userRoleRepository;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UserService(
    IMSSQLRepository<User, Guid> repository,
    IMSSQLRepository<UserRole, Guid> userRoleRepository,
    IMSSQLRepository<AuditLog, long> auditLogRepository,
    IApplicationDbContext context,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor
    )
        : base(repository, auditLogRepository, context, mapper, httpContextAccessor)
    {
        _repository = repository;
        _userRoleRepository = userRoleRepository;
        _context = context;
        _mapper = mapper;
    }

    public virtual async Task<Result<UserProfileDTO>> GetMeAsync(string oid)
    {
        Result<UserProfileDTO> result = new(false);

        try
        {
            var user = await _repository.GetAsync(u => u.EntraObjectId == oid);

            if (user == null)
                result.SetError("user not found", $"user with Id {oid}");

            var permissions = user.UserRoles
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Code)
                .Distinct().ToList();

            result.SetSuccess(new UserProfileDTO(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Phone,
                user.AvatarUrl,
                user.LastLoginAt,
                [.. user.UserRoles.Select(ur => _mapper.Map<RoleDTO>(ur.Role))],
                permissions), "Retrieved Successfully.");

        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Error while retrieving Base");
        }

        return result;
    }

    public virtual async Task<Result<UserRoleDTO>> AssignUserRoleAsync(Guid userId, Guid roleId)
    {
        Result<UserRoleDTO> result = new(false);

        try
        {

            if (userId == Guid.Empty)
            {
                result.SetError("Invalid OID", "The provided OID is null or empty.");
                return result;
            }
            else if (roleId == Guid.Empty)
            {
                result.SetError("Invalid Role ID", "The provided Role ID is empty.");
                return result;
            }

            var userRole = await _userRoleRepository.CreateAsync(new UserRole { Code = RandomGenerator.RandomString(10), RoleId = roleId, UserId = userId });
            await _context.SaveChangesAsync();

            result.SetSuccess(_mapper.Map<UserRoleDTO>(userRole), "User role assigned successfully.");

        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Error while assigning user role");
        }

        return result;
    }

    public override async Task<Result<TResponse>> CreateAsync<TResponse, TRequest>(TRequest request)
    {
        var result = await base.CreateAsync<TResponse, TRequest>(request);

        if (result.IsSuccess && request is CreateUserRequest createUserRequest && createUserRequest.RoleIds != null)
        {
            if (result.Content is UserDTO userDto)
            {
                foreach (var roleId in createUserRequest.RoleIds)
                {
                    await AssignUserRoleAsync(userDto.Id, roleId);
                }
            }
        }

        return result;
    }

    public override async Task<Result<bool>> UpdateAsync<TRequest>(Guid id, TRequest request)
    {
        var result = await base.UpdateAsync(id, request);

        if (result.IsSuccess && request is UpdateUserRequest updateUserRequest && updateUserRequest.RoleIds != null)
        {
            var existingRoles = await _userRoleRepository.GetAllAsync(ur => ur.UserId == id);
            var existingRoleIds = existingRoles.Select(ur => ur.RoleId).ToList();
            
            var rolesToAdd = updateUserRequest.RoleIds.Except(existingRoleIds).ToList();
            var rolesToRemove = existingRoleIds.Except(updateUserRequest.RoleIds).ToList();

            foreach (var roleId in rolesToAdd)
            {
                await AssignUserRoleAsync(id, roleId);
            }
            
            foreach (var roleId in rolesToRemove)
            {
                await RemoveUserRoleAsync(id, roleId);
            }
        }

        return result;
    }

    public virtual async Task<Result<UserRoleDTO>> RemoveUserRoleAsync(Guid userId, Guid roleId)
    {
        Result<UserRoleDTO> result = new(false);

        try
        {

            if (userId == Guid.Empty)
            {
                result.SetError("Invalid OID", "The provided OID is null or empty.");
                return result;
            }
            else if (roleId == Guid.Empty)
            {
                result.SetError("Invalid Role ID", "The provided Role ID is empty.");
                return result;
            }

            var userRole = await _userRoleRepository.DeleteAsync(x => x.UserId == userId && x.RoleId == roleId);
            await _context.SaveChangesAsync();

            // userRole is likely an IList<string> or similar if DeleteAsync with expression is called.
            // Oh wait, DeleteAsync(Expression) returns Task<IList<string>>
            // Actually, existing code says:
            // var userRole = await _userRoleRepository.DeleteAsync(x => x.UserId == userId && x.RoleId == roleId);
            // result.SetSuccess(_mapper.Map<UserRoleDTO>(userRole), "User role removed successfully.");
            
            result.SetSuccess(new UserRoleDTO(Guid.Empty, userId, roleId, null, null), "User role removed successfully.");

        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Error while removing user role");
        }

        return result;
    }
}