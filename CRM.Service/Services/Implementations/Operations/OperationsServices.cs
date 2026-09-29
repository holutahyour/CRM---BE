using CRM.Domain.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CRM.Services.Implementations;

/// <summary>
/// Generic CRUD for an Operations → Processing record log, refusing any create or update whose
/// request fails validation. Nothing validates requests before they reach a service in this API,
/// so the check lives here, where both the controller and the tests go through it.
/// </summary>
public abstract class ValidatedRecordService<TEntity> : MSSQLBaseService<TEntity, Guid>
    where TEntity : BaseEntity<Guid>
{
    protected IApplicationDbContext Context { get; }

    protected ValidatedRecordService(
        IMSSQLRepository<TEntity, Guid> baseRepository,
        IMSSQLRepository<AuditLog, long> auditLogRepository,
        IApplicationDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
        Context = context;
    }

    /// <summary>The reason the request is invalid, or null. <paramref name="id"/> is set on update.</summary>
    protected abstract Task<string?> ValidateAsync(object? request, Guid? id);

    public override async Task<Result<TResponse>> CreateAsync<TResponse, TRequest>(TRequest request)
    {
        var error = await ValidateAsync(request, null);
        if (error == null) return await base.CreateAsync<TResponse, TRequest>(request);

        var result = new Result<TResponse>(false);
        result.SetError(error, error);
        return result;
    }

    public override async Task<Result<bool>> UpdateAsync<TRequest>(Guid id, TRequest request)
    {
        var error = await ValidateAsync(request, id);
        if (error == null) return await base.UpdateAsync(id, request);

        var result = new Result<bool>(false);
        result.SetError(error, error);
        return result;
    }

    protected static string? Check<T>(IValidator<T> validator, T request)
    {
        var validation = validator.Validate(request);
        return validation.IsValid ? null : string.Join(" ", validation.Errors.Select(e => e.ErrorMessage));
    }

    /// <summary>
    /// A link is valid when it is empty or points at a live record in the caller's tenant — the
    /// global tenant/soft-delete filter makes anything else invisible here.
    /// </summary>
    protected async Task<bool> LinkExistsAsync<TLinked>(Guid? id) where TLinked : BaseEntity<Guid>
        => id is null || await Context.Set<TLinked>().AnyAsync(e => e.Id == id.Value);
}

public class ProcessingProductService : ValidatedRecordService<ProcessingProduct>, IProcessingProductService
{
    private static readonly CreateProcessingProductValidator Validator = new();

    public ProcessingProductService(
        IMSSQLRepository<ProcessingProduct, Guid> baseRepository,
        IMSSQLRepository<AuditLog, long> auditLogRepository,
        IApplicationDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
    }

    protected override async Task<string?> ValidateAsync(object? request, Guid? id)
    {
        if (request is not CreateProcessingProductRequest product) return null;
        var error = Check(Validator, product);
        if (error != null) return error;

        product.Name = product.Name.Trim();
        var name = product.Name.ToUpper();
        var taken = await Context.Set<ProcessingProduct>()
            .AnyAsync(p => p.Name.ToUpper() == name && p.Id != id);
        return taken ? $"A product named '{product.Name}' already exists." : null;
    }
}

public class OrderRequestService : ValidatedRecordService<OrderRequest>, IOrderRequestService
{
    private static readonly CreateOrderRequestValidator Validator = new();

    public OrderRequestService(
        IMSSQLRepository<OrderRequest, Guid> baseRepository,
        IMSSQLRepository<AuditLog, long> auditLogRepository,
        IApplicationDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
    }

    protected override Task<string?> ValidateAsync(object? request, Guid? id)
        => Task.FromResult(request is CreateOrderRequestRequest order ? Check(Validator, order) : null);
}

public class ProductionBatchService : ValidatedRecordService<ProductionBatch>, IProductionBatchService
{
    private static readonly CreateProductionBatchValidator Validator = new();

    public ProductionBatchService(
        IMSSQLRepository<ProductionBatch, Guid> baseRepository,
        IMSSQLRepository<AuditLog, long> auditLogRepository,
        IApplicationDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
    }

    protected override async Task<string?> ValidateAsync(object? request, Guid? id)
    {
        if (request is not CreateProductionBatchRequest batch) return null;
        var error = Check(Validator, batch);
        if (error != null) return error;

        if (!await LinkExistsAsync<OrderRequest>(batch.OrderRequestId)) return "Order request not found.";
        if (!await LinkExistsAsync<ProcessingProduct>(batch.ProductId)) return "Product not found.";
        return null;
    }
}

public class YieldEntryService : ValidatedRecordService<YieldEntry>, IYieldEntryService
{
    private static readonly CreateYieldEntryValidator Validator = new();

    public YieldEntryService(
        IMSSQLRepository<YieldEntry, Guid> baseRepository,
        IMSSQLRepository<AuditLog, long> auditLogRepository,
        IApplicationDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
    }

    protected override async Task<string?> ValidateAsync(object? request, Guid? id)
    {
        if (request is not CreateYieldEntryRequest entry) return null;
        var error = Check(Validator, entry);
        if (error != null) return error;

        if (!await LinkExistsAsync<Item>(entry.ProduceItemId)) return "Produce not found.";
        if (!await LinkExistsAsync<ProductionBatch>(entry.ProductionBatchId)) return "Production batch not found.";
        return null;
    }
}
