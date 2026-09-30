namespace CRM.Domain.DTOs;

// The Sales tabs post and read these shapes directly, so the property names
// here are the contract the frontend's record types are written against.
// `Date` is a DateOnly so it travels as "2026-06-01" — the UI sorts and slices
// the raw string, and a timestamp would break the day it renders in a
// timezone behind UTC.

// ── Daily Production ─────────────────────────────────────────────────────────

public class CreateSalesDailyProductionRequest
{
    public DateOnly Date { get; set; }
    public int OpeningBirds { get; set; }
    public int EggsMorning { get; set; }
    public int TotalEggs { get; set; }
    public int TotalEggsCrates { get; set; }
    public int Cracked { get; set; }
    public int Bad { get; set; }
    public int SmallEggs { get; set; }
}

public class SalesDailyProductionResponse : CreateSalesDailyProductionRequest
{
    public Guid Id { get; set; }
}

// ── Sales ────────────────────────────────────────────────────────────────────

public class CreateSalesRecordRequest
{
    public DateOnly Date { get; set; }
    public string Customer { get; set; } = "";

    /// <summary>Crates sold.</summary>
    public decimal Quantity { get; set; }

    /// <summary>Price per crate.</summary>
    public decimal Price { get; set; }

    public decimal Paid { get; set; }
    public string ModeOfPayment { get; set; } = "";
    public string? Remarks { get; set; }
}

public class SalesRecordResponse : CreateSalesRecordRequest
{
    public Guid Id { get; set; }
}

// ── Feed Cost ────────────────────────────────────────────────────────────────

public class CreateSalesFeedCostRequest
{
    public DateOnly Date { get; set; }
    public string FeedType { get; set; } = "";

    /// <summary>Bags purchased.</summary>
    public decimal Quantity { get; set; }

    public decimal CostPerBag { get; set; }
}

public class SalesFeedCostResponse : CreateSalesFeedCostRequest
{
    public Guid Id { get; set; }
}

// ── Stock ────────────────────────────────────────────────────────────────────

public class CreateSalesStockRecordRequest
{
    public DateOnly Date { get; set; }
    public int OpeningEggs { get; set; }
    public int OpeningCrates { get; set; }
    public int Produced { get; set; }
    public int Sold { get; set; }
    public int SoldCrates { get; set; }
    public int Loss { get; set; }
    public int LossCrates { get; set; }
}

public class SalesStockRecordResponse : CreateSalesStockRecordRequest
{
    public Guid Id { get; set; }
}

// ── Fresh Produce: Packhouse Intake ──────────────────────────────────────────

public class CreateProducePackhouseIntakeRequest
{
    public DateOnly Date { get; set; }
    public string ProduceType { get; set; } = "";
    public decimal GradeA { get; set; }
    public decimal GradeB { get; set; }
    public decimal GradeC { get; set; }
    public decimal Rejected { get; set; }
    public decimal QuantityHarvested { get; set; }
    public string? Remarks { get; set; }
}

public class ProducePackhouseIntakeResponse : CreateProducePackhouseIntakeRequest
{
    public Guid Id { get; set; }
}

// ── Fresh Produce: Sales ─────────────────────────────────────────────────────

public class CreateProduceSaleRequest
{
    public DateOnly Date { get; set; }
    public string Customer { get; set; } = "";
    public string? Location { get; set; }
    public string? ProduceType { get; set; }
    public string? Grade { get; set; }
    public string Category { get; set; } = "";

    /// <summary>Kilograms sold.</summary>
    public decimal Quantity { get; set; }

    public decimal PricePerKg { get; set; }
    public decimal Paid { get; set; }
    public string ModeOfPayment { get; set; } = "";
    public string PaymentStatus { get; set; } = "";
}

public class ProduceSaleResponse : CreateProduceSaleRequest
{
    public Guid Id { get; set; }
}

// ── Fresh Produce: Weekly Sales Summary ──────────────────────────────────────

public class CreateProduceWeeklySummaryRequest
{
    public DateOnly WeekStart { get; set; }
    public DateOnly WeekEnd { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalPaid { get; set; }
}

public class ProduceWeeklySummaryResponse : CreateProduceWeeklySummaryRequest
{
    public Guid Id { get; set; }
}
