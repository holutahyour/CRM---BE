using CRM.Base.Domain.Entities;
using CRM.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

/// <summary>
/// A scheduled production job — one row of the "Batch Production Schedule" sheet, from raw material
/// through processing to dispatch.
///
/// <see cref="OrderRequestId"/> and <see cref="ProductId"/> are optional links: the team records
/// batches for walk-in sales and samples that never had an order request or a coded product.
/// </summary>
[Table("ops_production_batches")]
public class ProductionBatch : TenantEntity<Guid>
{
    public Guid? OrderRequestId { get; set; }
    public Guid? ProductId { get; set; }

    public string? RawMaterialBatchId { get; set; }
    public string? CustomerCode { get; set; }
    public string? CustomerName { get; set; }
    public string? BatchCode { get; set; }
    public string ProductNames { get; set; } = "";
    public string? ProductCode { get; set; }

    public decimal? Quantity { get; set; }
    public string? QuantityUnit { get; set; }

    /// <summary>Per-product breakdowns the single quantity can't hold ("40g-200pcs…").</summary>
    public string? QuantityNotes { get; set; }

    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? LeadTime { get; set; }
    public string? WorkCenters { get; set; }
    public string? Operators { get; set; }
    public string? TaskDescription { get; set; }
    public ProcessingStatus Status { get; set; } = ProcessingStatus.NotStarted;
    public string? QualityChecks { get; set; }

    public DateOnly? DateSent { get; set; }
    public string? QuantitySent { get; set; }
    public string? LogisticsPersonnel { get; set; }
    public string? DeliveryStatus { get; set; }

    /// <summary>0–100.</summary>
    public decimal? OnTimeDeliveryPercent { get; set; }

    // Navigation
    public OrderRequest? OrderRequest { get; set; }
    public ProcessingProduct? Product { get; set; }
}
