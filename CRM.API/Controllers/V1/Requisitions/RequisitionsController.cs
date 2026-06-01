using CRM.Domain.Enums.Workflow;
using Microsoft.AspNetCore.Http;

namespace CRM.API.Controllers.v1;

[ApiVersion("1.0")]
public class RequisitionsController : MSSQLBaseController<Requisition, RequisitionResponse, Guid>
{
    private readonly IRequisitionService _service;
    private readonly IApprovalService _approvalService;

    public RequisitionsController(IRequisitionService service, IApprovalService approvalService) : base(service)
    {
        _service = service;
        _approvalService = approvalService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> CreateRequisition([FromForm] CreateRequisitionRequest request, IFormFile? file)
    {
        Stream? fileStream = file?.OpenReadStream();
        try
        {
            var response = await _service.CreateWithFileAsync(request, fileStream, file?.FileName, file?.ContentType);

            if (response.IsSuccess)
                return Ok(response);
            else
                return BadRequest(response);
        }
        finally
        {
            if (fileStream != null)
                await fileStream.DisposeAsync();
        }
    }

    [HttpPut]
    public async Task<ActionResult> UpdateRequisition(Guid id, [FromBody] UpdateRequisitionRequest request)
    {
        var response = await UpdateAsync(id, request);
        return response;
    }

    [HttpPut("{id}/approve")]
    public async Task<ActionResult> ApproveRequisition(Guid id)
    {
        var requestTime = DateTime.UtcNow;
        var response = await _service.ApproveAsync(id);
        var responseTime = DateTime.UtcNow;

        response.RequestTime = requestTime;
        response.ResponseTime = responseTime;


        if (response.IsSuccess)
            return Ok(response);
        else
            return BadRequest(response);
    }

    [HttpPut("{id}/reject")]
    public async Task<ActionResult> RejectRequisition(Guid id, [FromBody] RejectRequisitionRequest request)
    {

        var requestTime = DateTime.UtcNow;
        var response = await _service.RejectAsync(id, request.Reason);
        var responseTime = DateTime.UtcNow;

        response.RequestTime = requestTime;
        response.ResponseTime = responseTime;


        if (response.IsSuccess)
            return Ok(response);
        else
            return BadRequest(response);
    }

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateRequisitionRequest[] requests)
    {

        var response = await ImportAsync(requests);
        return response;
    }

    [HttpGet("{id}/approval-history")]
    public async Task<ActionResult> GetApprovalHistory(Guid id)
    {
        var tenantId = GetCurrentTenantId();
        var history = await _approvalService.GetHistoryAsync(WorkflowType.Requisition, id, tenantId);
        return Ok(new { isSuccess = true, content = history });
    }
}
