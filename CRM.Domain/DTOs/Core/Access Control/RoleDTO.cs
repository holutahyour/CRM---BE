namespace CRM.Domain.DTOs.Core;

public record RoleDTO(Guid Id, string Name, string Code, string? Description, bool IsSystem, bool IsActive, List<string> Permissions, List<Guid> PermissionIds);
public record CreateRoleRequest(string Name, string Code, string? Description, List<Guid> PermissionIds);
public record UpdateRoleRequest(string Name, string? Description, bool IsActive, List<Guid> PermissionIds);