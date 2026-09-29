using CRM.Base.Domain.Entities;
using CRM.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

/// <summary>
/// A customer's request for processing work — one row of the "ORDER REQUEST SHEET".
///
/// Products and volume stay free text: a single request routinely lists several products with
/// per-product sizes ("7 bottles tumeric (120g)…"), which no fixed column set captures.
/// </summary>
[Table("ops_order_requests")]
public class OrderRequest : TenantEntity<Guid>
{
    public DateOnly RequestDate { get; set; }
    public string? CustomerCode { get; set; }
    public string CustomerName { get; set; } = "";
    public string Products { get; set; } = "";
    public string? ActivitiesRequired { get; set; }
    public string? VolumeRequired { get; set; }
    public DateOnly? DeliveryDate { get; set; }
    public string? DeliveryLocation { get; set; }
    public string? ProductBatchNumber { get; set; }
    public ProcessingStatus Status { get; set; } = ProcessingStatus.NotStarted;
    public DateOnly? StartDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public string? Duration { get; set; }
}
