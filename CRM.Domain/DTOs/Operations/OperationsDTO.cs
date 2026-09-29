using CRM.Domain.Enums;

namespace CRM.Domain.DTOs;

// Operations → Processing posts and reads these shapes directly, so the property names here are
// the contract the frontend's record types are written against. Dates are DateOnly so they travel
// as "2026-06-01" (see SalesDTO for why a timestamp would shift the day in the browser).

// ── Products ─────────────────────────────────────────────────────────────────

public class CreateProcessingProductRequest
{
    public string Name { get; set; } = "";
    public string? ProductCode { get; set; }
    public string? Upc { get; set; }
    public string? Sku { get; set; }
    public string? RawMaterialId { get; set; }
    public string? ProcessingDuration { get; set; }
}

public class ProcessingProductResponse : CreateProcessingProductRequest
{
    public Guid Id { get; set; }
}

// ── Order Requests ───────────────────────────────────────────────────────────

public class CreateOrderRequestRequest
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

public class OrderRequestResponse : CreateOrderRequestRequest
{
    public Guid Id { get; set; }
}

// ── Production Batches ───────────────────────────────────────────────────────

public class CreateProductionBatchRequest
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
    public decimal? OnTimeDeliveryPercent { get; set; }
}

public class ProductionBatchResponse : CreateProductionBatchRequest
{
    public Guid Id { get; set; }
}

// ── Yield Entries ────────────────────────────────────────────────────────────

public class CreateYieldEntryRequest
{
    public DateOnly Date { get; set; }
    public Guid ProduceItemId { get; set; }
    public Guid? ProductionBatchId { get; set; }
    public string? Shift { get; set; }
    public decimal? InputQuantity { get; set; }
    public string? InputUnit { get; set; }
    public decimal? InputWeightKg { get; set; }
    public decimal? CutWeightKg { get; set; }
    public decimal? DehydratedWeightKg { get; set; }
    public decimal? GrindWeightKg { get; set; }
    public decimal? SecondGrindWeightKg { get; set; }
    public decimal? WasteKg { get; set; }
    public string? Notes { get; set; }
}

public class YieldEntryResponse : CreateYieldEntryRequest
{
    public Guid Id { get; set; }

    // Derived from the weights above — never stored (see YieldEntry).

    /// <summary>The weighed input, or the quantity when it was counted in kg; otherwise unknown.</summary>
    public decimal? InputKg =>
        InputWeightKg
        ?? (InputQuantity.HasValue && IsKg(InputUnit) ? InputQuantity : null);

    /// <summary>The last stage the run reached.</summary>
    public decimal? OutputKg => SecondGrindWeightKg ?? GrindWeightKg ?? DehydratedWeightKg ?? CutWeightKg;

    public decimal? YieldPercent => PercentOfInput(OutputKg);
    public decimal? WastePercent => PercentOfInput(WasteKg);

    private decimal? PercentOfInput(decimal? kg) =>
        kg.HasValue && InputKg is > 0 ? kg.Value / InputKg.Value * 100 : null;

    private static bool IsKg(string? unit) =>
        unit is not null && unit.Trim().Equals("kg", StringComparison.OrdinalIgnoreCase);
}

// ── Stock Cards ──────────────────────────────────────────────────────────────

/// <summary>One material on the Material Stock overview.</summary>
public class StockCardItemResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Sku { get; set; } = "";
    public string? CategoryName { get; set; }
    public string UnitType { get; set; } = "";
    public string? LocationName { get; set; }
    public decimal QuantityOnHand { get; set; }
}

/// <summary>One line of a stock card: the workbook's OPENING / RECEIVED / ISSUED OUT / CLOSING.</summary>
public class StockCardRow
{
    public Guid TransactionId { get; set; }
    public DateOnly Date { get; set; }
    public decimal Opening { get; set; }
    public decimal Received { get; set; }
    public decimal Issued { get; set; }
    public decimal Closing { get; set; }
    public string? WhereRequired { get; set; }
}

public class StockCardResponse
{
    public StockCardItemResponse Item { get; set; } = new();
    public List<StockCardRow> Rows { get; set; } = [];
}

/// <summary>Body of a stock-card Receive or Issue.</summary>
public class StockMovementRequest
{
    public DateOnly Date { get; set; }
    public decimal Quantity { get; set; }
    public string? WhereRequired { get; set; }
}
