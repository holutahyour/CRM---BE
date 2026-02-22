
using CRM.Base.Common.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("cor_states")]
public partial class State : BaseEntity<long>
{
    [MaxLength(500)]
    public required string Name { get; set; }

    [MaxLength(50)]
    public string Abbreviation { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Capital { get; set; } = string.Empty;

    public string Population { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Area { get; set; } = string.Empty;

    [MaxLength(50)]
    public required string CountryCode { get; set; }

    [ForeignKey("CountryId")]
    public Country Country { get; set; } = null!;

}

