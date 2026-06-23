namespace CRM.Domain.DTOs.Core;

public record ModuleDTO(
    Guid Id,
    string Name,
    string Code,
    string? Description,
    string? Version,
    Guid? CategoryId,
    string? CategoryName,
    bool IsActive);

public record TenantModuleDTO(
    Guid Id,
    Guid ModuleId,
    string ModuleName,
    string ModuleCode,
    string? CategoryName,
    bool IsActive,
    DateTime? ActivatedAt,
    DateTime? ExpiresAt);

/// <summary>Enable or disable a module for the current tenant.</summary>
public record ToggleModuleRequest(Guid ModuleId, bool Enabled);

/// <summary>Super-admin create/update of a global module catalog entry.</summary>
public record ModuleCatalogRequest(
    string Name,
    string Code,
    string? Description,
    string? Version,
    Guid? CategoryId,
    bool IsActive);
