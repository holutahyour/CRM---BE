namespace CRM.Domain.DTOs.Core;

public record RoleDTO(int Id, string Name, string Code, string? Description, bool IsSystem, bool IsActive, List<string> Permissions);
public record CreateRoleRequest(string Name, string Code, string? Description, List<int> PermissionIds);
public record UpdateRoleRequest(string Name, string? Description, bool IsActive, List<int> PermissionIds);