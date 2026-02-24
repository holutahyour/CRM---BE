namespace CRM.Domain.DTOs.Core;

public record UserRoleDTO(Guid Id, Guid UserId, Guid RoleId, UserDTO User, RoleDTO role);