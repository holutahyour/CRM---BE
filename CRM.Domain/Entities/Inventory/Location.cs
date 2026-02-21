using CRM.Base.Domain.Entities;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities.Inventory;

public class Location : TenantEntity<Guid>
{
    public string Name { get; set; } = "";
    public string? Code { get; set; }
    public LocationType Type { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Capacity { get; set; }
    public bool TemperatureControl { get; set; }

    // Navigation
    public ICollection<ItemLocation> ItemLocations { get; set; } = [];
}