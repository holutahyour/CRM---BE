namespace CRM.Domain;

public partial class StateResponse : CreateStateRequest
{
    public long Id { get; set; }

    public string Code { get; set; }
}

public partial class CreateStateRequest
{
    public required string Name { get; set; }

    public string Abbreviation { get; set; }

    public string Capital { get; set; }

    public string Population { get; set; }

    public string Area { get; set; }

    public required string CountryCode { get; set; }
}

public partial class UpdateStateRequest
{
    public required long Id { get; set; }
    public required string Name { get; set; }

    public string Abbreviation { get; set; }

    public string Capital { get; set; }

    public string Population { get; set; }

    public string Area { get; set; }
}

