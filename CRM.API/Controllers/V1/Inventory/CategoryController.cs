namespace CRM.API.Controllers.v1;

[ApiVersion("1.0")]
public class CategoriesController : MSSQLBaseController<Category, CategoryResponse, Guid>
{
    private readonly ICategoryService _service;

    public CategoriesController(ICategoryService service) : base(service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {

        var response = await CreateAsync(request);

        return response;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest request)
    {

        var response = await UpdateAsync(id, request);

        return response;
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveCategory(Guid id)
    {

        var response = await RemoveAsync(id);

        return response;
    }

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateCategoryRequest[] requests)
    {

        var response = await ImportAsync(requests);

        return response;
    }
}
