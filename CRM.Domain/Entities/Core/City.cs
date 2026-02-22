
using CRM.Base.Common.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("cor_cities")]
public partial class City : BaseEntity<long>
{
    [MaxLength(500)]
    public required string Name { get; set; }

    public string Population { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Area { get; set; } = string.Empty;

    [MaxLength(50)]
    public string CountryCode { get; set; } = string.Empty;

    [ForeignKey("CountryId")]
    public Country Country { get; set; } = null!;

    [MaxLength(50)]
    public string StateCode { get; set; } = string.Empty;

    [ForeignKey("StateId")]
    public State State { get; set; } = null!;
}

