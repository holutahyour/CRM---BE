namespace CRM.Base.Domain.Entities;

public abstract class TenantEntity<T> : BaseEntity<T>, ITenantEntity
{
    public Guid TenantId { get; set; }
}

