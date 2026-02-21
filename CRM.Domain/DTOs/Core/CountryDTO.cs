namespace CRM.Domain;

public partial class CountryResponse : CreateCountryRequest
{
    public long Id { get; set; }
    public string Code { get; set; }
}

public partial class CreateCountryRequest
{
    public required string CountryName { get; set; }

    public string Capital { get; set; }
    public string CountryCode { get; set; }
    public string Currency { get; set; }

    public string Flag { get; set; }

    public string PhoneCode { get; set; }

    public string TimeZone { get; set; }

    public string Continent { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string GoogleMaps { get; set; }
}

public partial class UpdateCountryRequest
{
    public required long Id { get; set; }
    public required string CountryName { get; set; }

    public string Capital { get; set; }
    public string CountryCode { get; set; }
    public string Currency { get; set; }

    public string Flag { get; set; }

    public string PhoneCode { get; set; }

    public string TimeZone { get; set; }

    public string Continent { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string GoogleMaps { get; set; }
}

