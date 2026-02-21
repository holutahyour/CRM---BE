namespace CRM.Domain;

public partial class ParameterValueResponse : CreateParameterValueRequest
{
    public long Id { get; set; }
    public string Code { get; set; }

}

public partial class CreateParameterValueRequest
{
    public required string ParameterCode { get; set; }
    public required string Value { get; set; }
    public required string UserCode { get; set; }
    public required string HierarchyInstanceCode { get; set; }
}


public partial class UpdateParameterValueRequest
{
    public required long Id { get; set; }
    public required string ParameterCode { get; set; }
    public required string Value { get; set; }
    public required string UserCode { get; set; }
    public required string HierarchyInstanceCode { get; set; }
}

