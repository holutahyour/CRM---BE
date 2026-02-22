namespace CRM.Domain;

public partial class CountryResponse : CreateCountryRequest
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
}

public partial class CreateCountryRequest
{
    public required string CountryName { get; set; }

    public string Capital { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;

    public string Flag { get; set; } = string.Empty;

    public string PhoneCode { get; set; } = string.Empty;

    public string TimeZone { get; set; } = string.Empty;

    public string Continent { get; set; } = string.Empty;
    public string Latitude { get; set; } = string.Empty;
    public string Longitude { get; set; } = string.Empty;
    public string GoogleMaps { get; set; } = string.Empty;
}

public partial class UpdateCountryRequest
{
    public required long Id { get; set; }
    public required string CountryName { get; set; }

    public string Capital { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;

    public string Flag { get; set; } = string.Empty;

    public string PhoneCode { get; set; } = string.Empty;

    public string TimeZone { get; set; } = string.Empty;

    public string Continent { get; set; } = string.Empty;
    public string Latitude { get; set; } = string.Empty;
    public string Longitude { get; set; } = string.Empty;
    public string GoogleMaps { get; set; } = string.Empty;
}

