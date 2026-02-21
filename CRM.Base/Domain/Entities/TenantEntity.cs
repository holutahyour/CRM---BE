namespace CRM.Base.Domain.Entities;

public abstract class TenantEntity<T> : BaseEntity<T>
{
    public int TenantId { get; set; }
}

