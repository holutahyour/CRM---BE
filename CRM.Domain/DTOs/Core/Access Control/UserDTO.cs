namespace CRM.Domain.DTOs.Core;

public record UserDTO(Guid Id, string Email, string? FirstName, string? LastName, string? Phone, bool IsActive, DateTime? LastLoginAt, List<string> Roles);
public record UserProfileDTO(Guid Id, string Email, string? FirstName, string? LastName, string? Phone, string? AvatarUrl, DateTime? LastLoginAt, List<RoleDTO> Roles, List<string> Permissions);
public record CreateUserRequest(string? FirstName, string? LastName, string? Phone, List<int>? RoleIds);
public record UpdateUserRequest(string? FirstName, string? LastName, string? Phone, bool IsActive, List<int>? RoleIds);