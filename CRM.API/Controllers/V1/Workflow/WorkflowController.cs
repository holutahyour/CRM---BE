using CRM.Domain.Constants;
using CRM.Domain.Enums.Workflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/workflows")]
public class WorkflowController : ControllerBase
{
    private readonly IWorkflowService _workflowService;

    protected Guid GetCurrentTenantId() =>
        HttpContext.Items["TenantId"] is Guid id ? id : Guid.Empty;

    public WorkflowController(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }

    [Authorize(Policy = "WorkflowTemplatesView")]
    [HttpGet]
    public async Task<ActionResult> GetAllTemplates()
    {
        var tenantId = GetCurrentTenantId();
        var templates = await _workflowService.GetAllTemplatesAsync(tenantId);
        return Ok(templates);
    }

    [Authorize(Policy = "WorkflowTemplatesView")]
    [HttpGet("{type:int}")]
    public async Task<ActionResult> GetActiveTemplate(int type)
    {
        var tenantId = GetCurrentTenantId();
        var workflowType = (WorkflowType)type;
        var template = await _workflowService.GetActiveTemplateAsync(workflowType, tenantId);
        if (template is null)
            return NotFound();
        return Ok(template);
    }

    [Authorize(Policy = "WorkflowTemplatesManage")]
    [HttpPost]
    public async Task<ActionResult> CreateTemplate([FromBody] CreateWorkflowTemplateRequest request)
    {
        var tenantId = GetCurrentTenantId();
        var template = await _workflowService.CreateTemplateAsync(request, tenantId);
        return Ok(template);
    }

    [Authorize(Policy = "WorkflowTemplatesManage")]
    [HttpPut("{id:guid}/steps")]
    public async Task<ActionResult> UpsertSteps(Guid id, [FromBody] List<WorkflowStepRequest> steps)
    {
        var tenantId = GetCurrentTenantId();
        await _workflowService.UpsertStepsAsync(id, steps, tenantId);
        return NoContent();
    }
}
