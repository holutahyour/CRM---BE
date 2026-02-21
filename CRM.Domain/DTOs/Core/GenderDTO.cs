namespace CRM.Domain;

public partial class GenderResponse : CreateGenderRequest
{
    public long Id { get; set; }
    public string Code { get; set; }

}

public partial class CreateGenderRequest
{
    public required string Name { get; set; }
    public string Description { get; set; }
}

public partial class UpdateGenderRequest
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; }
}

