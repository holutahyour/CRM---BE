using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("fr_parameter_definitions")]
public partial class ParameterDefinition
{
    [Key]
    [StringLength(50)]
    public string ParameterCode { get; set; } = null!;

    [Required]
    [StringLength(150)]
    public string DisplayName { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [StringLength(50)]
    public string DataType { get; set; } = null!;  // Integer, Boolean, Decimal, String, Enum, Date, ...

    public bool IsRequired { get; set; } = true;

    [StringLength(100)]
    public string? MinValue { get; set; }

    [StringLength(100)]
    public string? MaxValue { get; set; }

    public string? AllowedValues { get; set; }     // comma-separated or JSON

    [StringLength(200)]
    public string? ValidationRegex { get; set; }

    public string? DefaultValue { get; set; }

    [StringLength(50)]
    public string? Scope { get; set; }             // GlobalOnly, User, Hierarchy, ...

    [StringLength(50)]
    public string? ActivityCode { get; set; }      // optional link to sac_activity_codes

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    [Required]
    [StringLength(50)]
    public string CreatedBy { get; set; } = null!;

    public DateTime? ModifiedOn { get; set; }

    [StringLength(50)]
    public string? ModifiedBy { get; set; }

    // Optional navigation property if you add FK
    // public virtual ActivityCode? ActivityCodeNavigation { get; set; }
}