
using CRM.Base.Common.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

[Table("cor_countries")]
public partial class Country : BaseEntity<long>
{
    [MaxLength(500)]
    public required string CountryName { get; set; }

    [MaxLength(100)]
    public string Capital { get; set; }

    [MaxLength(50)]
    public string CountryCode { get; set; }

    [MaxLength(50)]
    public string Currency { get; set; }

    [MaxLength(50)]
    public string Flag { get; set; }

    [MaxLength(50)]
    public string PhoneCode { get; set; }

    [MaxLength(50)]
    public string TimeZone { get; set; }

    [MaxLength(50)]
    public string Continent { get; set; }

    [MaxLength(50)]
    public string Latitude { get; set; }

    [MaxLength(50)]
    public string Longitude { get; set; }

    [MaxLength(500)]
    public string GoogleMaps { get; set; }


}

