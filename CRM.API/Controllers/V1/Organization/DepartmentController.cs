using Asp.Versioning;
using CRM.Base.Common.Presentation;
using CRM.Domain.DTOs;
using CRM.Domain.Entities;
using CRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.v1;

[ApiVersion("1.0")]
public class DepartmentsController : MSSQLBaseController<Department, DepartmentResponse, Guid>
{
    private readonly IDepartmentService _service;

    public DepartmentsController(IDepartmentService service) : base(service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> CreateDepartment([FromBody] CreateDepartmentRequest request)
    {

        var response = await CreateAsync(request);

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateDepartment(Guid id, [FromBody] UpdateDepartmentRequest request)
    {

        var response = await UpdateAsync(id, request);

        return Ok(response);
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveDepartment(Guid id)
    {

        var response = await RemoveAsync(id);

        return Ok(response);
    }

    [HttpPost("import")]
    public async Task<ActionResult> Import([FromBody] CreateDepartmentRequest[] requests)
    {

        var response = await ImportAsync(requests);

        return Ok(response);
    }
}
