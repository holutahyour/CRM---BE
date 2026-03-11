namespace CRM.API.Controllers.v1;

[ApiVersion("1.0")]
public class MonthlyreportsController : MSSQLBaseController<MonthlyReport, MonthlyReportResponse, Guid>
{
    private readonly IMonthlyReportService _service;

    public MonthlyreportsController(IMonthlyReportService service) : base(service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> CreateMonthlyReport([FromBody] CreateMonthlyReportRequest request)
    {

        var response = await CreateAsync(request);

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateMonthlyReport(Guid id, [FromBody] UpdateMonthlyReportRequest request)
    {

        var response = await UpdateAsync(id, request);

        return Ok(response);
    }

    //[HttpDelete]
    //public async Task<ActionResult> RemoveMonthlyReport(Guid id)
    //{

    //    var response = await RemoveAsync(id);

    //    return Ok(response);
    //}

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateMonthlyReportRequest[] requests)
    {

        var response = await ImportAsync(requests);

        return Ok(response);
    }
}

