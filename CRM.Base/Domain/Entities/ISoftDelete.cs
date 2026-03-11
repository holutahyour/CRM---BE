namespace CRM.Base.Domain.Entities;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}
