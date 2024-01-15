using WIGOS.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using WIGOS.Web.Application.Core;
using WIGOS.Web.Application.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using WIGOS.Web.Application.WIGOS.AbstractEnvironmentalMonitoringFacilities.AbstractEnvironmentalMonitoringFacility;
using WIGOS.Web.Application.WIGOS.AbstractEnvironmentalMonitoringFacilities.ObservingFacility;
using Microsoft.AspNetCore.Mvc;

namespace WIGOS.Web.API.Controllers.WIGOS
{
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
            return HandleResult(await Mediator.Send(new Create.Command { ObservingFacility = observingFacility }));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditObjectItem(Guid id, ObservingFacility observingFacility)
        {
            observingFacility.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command { ObservingFacility = observingFacility }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteObservingFacility(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }
    }
}