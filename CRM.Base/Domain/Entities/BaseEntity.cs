using System.ComponentModel.DataAnnotations;

namespace CRM.Base.Common.Domain.Entities;

public class BaseEntity<T> : AuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public T Id { get; set; } = default!;

    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
}
