namespace CRM.Domain.DTOs.Core;

public record PermissionDTO(Guid Id, string Name, string Code, string? Description, string? ModuleCode);
