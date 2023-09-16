using Microsoft.AspNetCore.Mvc;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.WIGOS.AbstractEnvironmentalMonitoringFacility;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;

namespace C2IEDM.Web.API.Controllers.WIGOS;

public class ObservingFacilitiesController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetObservingFacilities([FromQuery] PagingParams param)
    {
        return HandlePagedResult(await Mediator.Send(new List.Query
        {
            Category = AbstractEnvironmentalMonitoringFacilityCategory.ObservingFacility,
            Params = param
        }));
    }

    [HttpPost]
    public async Task<IActionResult> CreateObservingFacility(ObservingFacility observingFacility)
    {
        return HandleResult(await Mediator.Send(new Application.WIGOS.ObservingFacility.Create.Command { ObservingFacility = observingFacility }));
    }
}