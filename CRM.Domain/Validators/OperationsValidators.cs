using CRM.Domain.DTOs;
using FluentValidation;

namespace CRM.Domain.Validators;

// Shape rules for the Operations → Processing requests. Rules that need the database (does the
// linked record exist in this tenant, is a name taken) live in the services.

public class CreateProcessingProductValidator : AbstractValidator<CreateProcessingProductRequest>
{
    public CreateProcessingProductValidator()
    {
        RuleFor(x => x.Name).Must(NotBlank).WithMessage("Product name is required.").MaximumLength(200);
        RuleFor(x => x.ProductCode).MaximumLength(100);
        RuleFor(x => x.Upc).MaximumLength(50);
        RuleFor(x => x.Sku).MaximumLength(50);
        RuleFor(x => x.RawMaterialId).MaximumLength(50);
        RuleFor(x => x.ProcessingDuration).MaximumLength(100);
    }

    private static bool NotBlank(string? s) => !string.IsNullOrWhiteSpace(s);
}

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequestRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.RequestDate).NotEqual(default(DateOnly)).WithMessage("Request date is required.");
        RuleFor(x => x.CustomerName).Must(s => !string.IsNullOrWhiteSpace(s))
            .WithMessage("Customer name is required.").MaximumLength(200);
        RuleFor(x => x.Products).Must(s => !string.IsNullOrWhiteSpace(s))
            .WithMessage("Products are required.").MaximumLength(2000);
        RuleFor(x => x.CustomerCode).MaximumLength(50);
        RuleFor(x => x.ActivitiesRequired).MaximumLength(2000);
        RuleFor(x => x.VolumeRequired).MaximumLength(2000);
        RuleFor(x => x.DeliveryLocation).MaximumLength(500);
        RuleFor(x => x.ProductBatchNumber).MaximumLength(100);
        RuleFor(x => x.Duration).MaximumLength(100);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.DueDate).GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.DueDate.HasValue)
            .WithMessage("Due date must be on or after the start date.");
    }
}

public class CreateProductionBatchValidator : AbstractValidator<CreateProductionBatchRequest>
{
    public CreateProductionBatchValidator()
    {
        RuleFor(x => x.ProductNames).Must(s => !string.IsNullOrWhiteSpace(s))
            .WithMessage("Product names are required.").MaximumLength(2000);
        RuleFor(x => x.RawMaterialBatchId).MaximumLength(100);
        RuleFor(x => x.CustomerCode).MaximumLength(50);
        RuleFor(x => x.CustomerName).MaximumLength(200);
        RuleFor(x => x.BatchCode).MaximumLength(200);
        RuleFor(x => x.ProductCode).MaximumLength(500);
        RuleFor(x => x.QuantityUnit).MaximumLength(30);
        RuleFor(x => x.QuantityNotes).MaximumLength(2000);
        RuleFor(x => x.LeadTime).MaximumLength(100);
        RuleFor(x => x.WorkCenters).MaximumLength(500);
        RuleFor(x => x.Operators).MaximumLength(500);
        RuleFor(x => x.TaskDescription).MaximumLength(2000);
        RuleFor(x => x.QualityChecks).MaximumLength(500);
        RuleFor(x => x.QuantitySent).MaximumLength(2000);
        RuleFor(x => x.LogisticsPersonnel).MaximumLength(200);
        RuleFor(x => x.DeliveryStatus).MaximumLength(100);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.");
        RuleFor(x => x.OnTimeDeliveryPercent).InclusiveBetween(0, 100)
            .WithMessage("On-time delivery must be between 0 and 100%.");
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("End date must be on or after the start date.");
    }
}

public class CreateYieldEntryValidator : AbstractValidator<CreateYieldEntryRequest>
{
    public CreateYieldEntryValidator()
    {
        RuleFor(x => x.Date).NotEqual(default(DateOnly)).WithMessage("Date is required.");
        RuleFor(x => x.ProduceItemId).NotEmpty().WithMessage("Produce is required.");
        RuleFor(x => x.Shift).MaximumLength(20);
        RuleFor(x => x.InputUnit).MaximumLength(30);
        RuleFor(x => x.Notes).MaximumLength(1000);
        RuleFor(x => x.InputQuantity).GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.");
        RuleFor(x => x.InputWeightKg).GreaterThanOrEqualTo(0).WithMessage("Input weight cannot be negative.");
        RuleFor(x => x.CutWeightKg).GreaterThanOrEqualTo(0).WithMessage("Cut weight cannot be negative.");
        RuleFor(x => x.DehydratedWeightKg).GreaterThanOrEqualTo(0).WithMessage("Dehydrated weight cannot be negative.");
        RuleFor(x => x.GrindWeightKg).GreaterThanOrEqualTo(0).WithMessage("Grind weight cannot be negative.");
        RuleFor(x => x.SecondGrindWeightKg).GreaterThanOrEqualTo(0).WithMessage("Second grind weight cannot be negative.");
        RuleFor(x => x.WasteKg).GreaterThanOrEqualTo(0).WithMessage("Waste cannot be negative.");
    }
}

public class StockMovementValidator : AbstractValidator<StockMovementRequest>
{
    public StockMovementValidator()
    {
        RuleFor(x => x.Date).NotEqual(default(DateOnly)).WithMessage("Date is required.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        RuleFor(x => x.WhereRequired).MaximumLength(500);
    }
}
