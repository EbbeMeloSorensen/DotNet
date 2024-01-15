using WIGOS.Web.Application.Core;
using WIGOS.Web.Application.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using WIGOS.Web.Application.WIGOS.AbstractEnvironmentalMonitoringFacilities.AbstractEnvironmentalMonitoringFacility;
using Microsoft.AspNetCore.Mvc;

namespace WIGOS.Web.API.Controllers.WIGOS
{
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
}