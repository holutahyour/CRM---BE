using CRM.Base.Common.Domain.Common;
using CRM.Base.Common.Services.Interface;
using CRM.Domain.DTOs.Core;
using CRM.Domain.Entities;

namespace CRM.Services.Interfaces;

public interface IModuleService : IMSSQLBaseService<Module, Guid>
{
    /// <summary>Global module catalog (with category names).</summary>
    Task<Result<IList<ModuleDTO>>> GetCatalogAsync();

    /// <summary>Modules enabled/disabled for the current tenant.</summary>
    Task<Result<IList<TenantModuleDTO>>> GetTenantModulesAsync();

    /// <summary>Enable or disable a module for the current tenant.</summary>
    Task<Result<bool>> ToggleAsync(ToggleModuleRequest request);
}
