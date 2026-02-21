using CRM.Base.Common.Domain.Entities;
using CRM.Domain.Enums;
using Microsoft.Graph.Models;

namespace CRM.Domain.Entities;

public class Tenant : BaseEntity<Guid>  // NOT TenantEntity — this IS the tenant
{
    public string Name { get; set; } = "";
    public string? Code { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? IndustryType { get; set; }
    public string? Address { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? SubscriptionPlan { get; set; }
    public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.Trial;
    public DateTime? TrialEndDate { get; set; }

    // Navigation
    public ICollection<User> Users { get; set; } = [];
    public ICollection<TenantModule> TenantModules { get; set; } = [];
}
