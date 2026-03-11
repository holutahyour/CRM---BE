using CRM.Base.Domain.Entities;

namespace CRM.Base.Common.Domain.Entities;

public class AuditableEntity : ISoftDelete
{
    public DateTime CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime LastModifiedOn { get; set; }

    public string? LastModifiedBy { get; set; }

    public bool IsDeleted { get; set; } = false;

    //[Timestamp]
    //public byte[] RowVersion { get; set; } = [];

}
