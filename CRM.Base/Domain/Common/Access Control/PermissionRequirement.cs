using Microsoft.AspNetCore.Authorization;

namespace CRM.Base.Domain.Common;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
