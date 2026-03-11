namespace CRM.Domain.DTOs;


public partial class ItemLocationResponse : CreateItemLocationRequest
{
    public Guid Id { get; set; }

}

public class CreateItemLocationRequest
{
    public int ItemLocationId { get; set; }
    public int LocationId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Reserved { get; set; }
    public decimal Damaged { get; set; }
}

public class UpdateItemLocationRequest
{
    public decimal Quantity { get; set; }
    public decimal Reserved { get; set; }
    public decimal Damaged { get; set; }
}
