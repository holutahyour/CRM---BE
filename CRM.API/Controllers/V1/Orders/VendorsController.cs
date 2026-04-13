namespace CRM.API.Controllers.v1;

[ApiVersion("1.0")]
public class VendorsController : MSSQLBaseController<Vendor, VendorResponse, Guid>
{
    private readonly IVendorService _service;

    public VendorsController(IVendorService service) : base(service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> CreateVendor([FromBody] CreateVendorRequest request)
    {
        return await CreateAsync(request);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateVendor(Guid id, [FromBody] UpdateVendorRequest request)
    {
        return await UpdateAsync(id, request);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveVendor(Guid id)
    {
        return await RemoveAsync(id);
    }
}
