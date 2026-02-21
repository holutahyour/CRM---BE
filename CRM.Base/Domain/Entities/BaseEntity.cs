using System.ComponentModel.DataAnnotations;

namespace CRM.Base.Common.Domain.Entities;

public class BaseEntity<T> : AuditableEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public T Id { get; set; }

    [Key]
    [MaxLength(50)]
    public string Code { get; set; }
}
