
using CRM.Base.Common.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("cor_genders")]
public partial class Gender : BaseEntity<long>
{
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }

}

