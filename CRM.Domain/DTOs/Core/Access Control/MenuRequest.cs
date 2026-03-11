namespace CRM.Domain.DTOs.Core;

public record CreateMenuRequest(string Name, string Label, string? Icon, string? Route, int Position, Guid? ParentId, List<Guid> PermissionIds);
public record UpdateMenuRequest(string Name, string Label, string? Icon, string? Route, int Position, Guid? ParentId, List<Guid> PermissionIds);
