using CRM.Domain.Enums;

namespace CRM.Domain.DTOs.Core;

public record UserDTO(Guid Id, string Email, string? FirstName, string? LastName, string? Phone, bool IsActive, DateTime? LastLoginAt, List<string> Roles);
public record UserProfileDTO(Guid Id, string Email, string? FirstName, string? LastName, string? Phone, string? AvatarUrl, DateTime? LastLoginAt, List<RoleDTO> Roles, List<string> Permissions);
public record CreateUserRequest(string? FirstName, string? LastName, string? Phone, List<int>? RoleIds);
public record UpdateUserRequest(string? FirstName, string? LastName, string? Phone, bool IsActive, List<int>? RoleIds);
public record UpdateUserBasicRequest(bool? Onboarded, UserStatus? Status);
public record GetUnauthorizedLogsRequest(Guid UserId);
public record UserSummary(int Total, int Onboarded, int NotOnboarded, int UnauthorizedActions);