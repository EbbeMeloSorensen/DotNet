using Microsoft.AspNetCore.Mvc;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using C2IEDM.Web.Application.WIGOS.AbstractEnvironmentalMonitoringFacilities.AbstractEnvironmentalMonitoringFacility;

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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAbstractEnvironmentalMonitoringFacility(Guid id)
    {
        return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
    }
}