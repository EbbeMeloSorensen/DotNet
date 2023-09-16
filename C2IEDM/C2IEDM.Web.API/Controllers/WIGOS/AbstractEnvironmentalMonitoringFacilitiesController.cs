using Microsoft.AspNetCore.Mvc;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.WIGOS.AbstractEnvironmentalMonitoringFacility;

namespace C2IEDM.Web.API.Controllers.WIGOS;

public class AbstractEnvironmentalMonitoringFacilitiesController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAbstractEnvironmentalMonitoringFacilities([FromQuery] PagingParams param)
    {
        return HandlePagedResult(await Mediator.Send(new List.Query
        {
            Category = AbstractEnvironmentalMonitoringFacilityCategory.AbstractEnvironmentalMonitoringFacility,
            Params = param
        }));
    }
}