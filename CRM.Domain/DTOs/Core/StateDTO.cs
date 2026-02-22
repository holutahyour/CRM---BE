namespace CRM.Domain;

public partial class StateResponse : CreateStateRequest
{
    public long Id { get; set; }

    public string Code { get; set; } = string.Empty;
}

public partial class CreateStateRequest
{
    public required string Name { get; set; }

    public string Abbreviation { get; set; } = string.Empty;

    public string Capital { get; set; } = string.Empty;

    public string Population { get; set; } = string.Empty;

    public string Area { get; set; } = string.Empty;

    public required string CountryCode { get; set; }
}

public partial class UpdateStateRequest
{
    public required long Id { get; set; }
    public required string Name { get; set; }

    public string Abbreviation { get; set; } = string.Empty;

    public string Capital { get; set; } = string.Empty;

    public string Population { get; set; } = string.Empty;

    public string Area { get; set; } = string.Empty;
}

