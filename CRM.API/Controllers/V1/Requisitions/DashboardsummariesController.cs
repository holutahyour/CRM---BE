namespace CRM.API.Controllers.v1;

[ApiVersion("1.0")]
public class DashboardSummariesController : MSSQLBaseController<DashboardSummary, DashboardSummaryResponse, Guid>
{
    private readonly IDashboardSummaryService _service;

    public DashboardSummariesController(IDashboardSummaryService service) : base(service)
    {
        _service = service;
    }
}
