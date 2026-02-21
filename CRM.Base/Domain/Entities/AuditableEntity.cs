namespace CRM.Base.Common.Domain.Entities;

public class AuditableEntity
{
    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public string? CreatedBy { get; set; }

    public DateTime LastModifiedOn { get; set; }

    public string? LastModifiedBy { get; set; }

    public bool IsDeleted { get; set; } = false;

    public byte[] RowVersion { get; set; } = [];

}
