using Asp.Versioning;
using CRM.Base.Common.Presentation;
using CRM.Domain;
using CRM.Domain.Entities;
using CRM.Services.Interfaces;

namespace CRM.API.Controllers.v1;

[ApiVersion("1.0")]
public class CountriesController : MSSQLBaseController<Country, CountryResponse, long>
{
    private readonly ICountryService _service;

    public CountriesController(ICountryService service) : base(service)
    {
        _service = service;
    }

    //[HttpPost]
    //public async Task<ActionResult> CreateCountry([FromBody] CreateCountryRequest request)
    //{

    //    var response = await CreateAsync(request);

    //    return Ok(response);
    //}

    //[HttpPut]
    //public async Task<ActionResult> UpdateCountry(long id, [FromBody] UpdateCountryRequest request)
    //{

    //    var response = await UpdateAsync(id, request);

    //    return Ok(response);
    //}

    //[HttpDelete]
    //public async Task<ActionResult> RemoveCountry(long id)
    //{

    //    var response = await RemoveAsync(id);

    //    return Ok(response);
    //}

}
