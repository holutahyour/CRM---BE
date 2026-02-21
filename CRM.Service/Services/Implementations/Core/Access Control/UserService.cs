using CRM.Base.Common.Domain.Common;
using CRM.Base.Common.Domain.Entities;
using CRM.Base.Common.Repositories;
using CRM.Base.Common.Repositories.Interfaces;
using CRM.Base.Common.Services.Implementation;
using CRM.Domain.DTOs.Core;
using Microsoft.AspNetCore.Http;

public class UserService : MSSQLBaseService<User, Guid>, IUserService
{
    private readonly IMSSQLRepository<User, Guid> _repository;
    private readonly IMapper _mapper;

    public UserService(
    IMSSQLRepository<User, Guid> repository,
    IMSSQLRepository<AuditLog, long> auditLogRepository,
    IApplicationDbContext context,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor
    )
        : base(repository, auditLogRepository, context, mapper, httpContextAccessor)
    {
        _repository = repository;
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
}