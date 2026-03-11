using CRM.Domain.Enums;

namespace CRM.Domain.DTOs;


public partial class LocationResponse : CreateLocationRequest
{
    public Guid Id { get; set; }

}

public class CreateLocationRequest
{
    public string Name { get; set; } = "";
    public string? Code { get; set; }
    public LocationType Type { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Capacity { get; set; }
    public bool TemperatureControl { get; set; }
}

public class UpdateLocationRequest
{
    public decimal? Capacity { get; set; }
    public bool TemperatureControl { get; set; }
}
