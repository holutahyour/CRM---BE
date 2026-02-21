namespace CRM.Domain;

public partial class EthnicityResponse : CreateEthnicityRequest
{
    public long Id { get; set; }

    public string Code { get; set; }

}

public partial class CreateEthnicityRequest
{
    public required string Name { get; set; }

    public required string Description { get; set; }
}

public partial class UpdateEthnicityRequest
{
    public required long Id { get; set; }

    public required string Name { get; set; }

    public string Description { get; set; }
}

