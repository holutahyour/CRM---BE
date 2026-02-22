namespace CRM.Domain;

public partial class CityResponse : CreateCityRequest
{
    public long Id { get; set; }

    public string Code { get; set; } = string.Empty;

}

public partial class CreateCityRequest
{
    public required string Name { get; set; }

    public string Population { get; set; } = string.Empty;

    public string Area { get; set; } = string.Empty;

    public required string CountryCode { get; set; }

    public required string StateCode { get; set; }

}

public partial class UpdateCityRequest
{
    public required long Id { get; set; }
    public required string Name { get; set; }

    public string Population { get; set; } = string.Empty;

    public string Area { get; set; } = string.Empty;
}

