
using CRM.Base.Common.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("cor_cities")]
public partial class City : BaseEntity<long>
{
    [MaxLength(500)]
    public required string Name { get; set; }

    public string Population { get; set; }

    [MaxLength(100)]
    public string Area { get; set; }

    [MaxLength(50)]
    public string CountryCode { get; set; }

    [ForeignKey("CountryCode")]
    public Country Country { get; set; }

    [MaxLength(50)]
    public string StateCode { get; set; }

    [ForeignKey("StateCode")]
    public State State { get; set; }
}

