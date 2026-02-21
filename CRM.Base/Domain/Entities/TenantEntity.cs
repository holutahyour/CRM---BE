namespace CRM.Base.Domain.Entities;

public abstract class TenantEntity<T> : BaseEntity<T>
{
    public Guid TenantId { get; set; }
}

