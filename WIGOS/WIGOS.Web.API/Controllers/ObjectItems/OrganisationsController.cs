using WIGOS.Domain.Entities.ObjectItems.Organisations;
using WIGOS.Web.Application.Core;
using WIGOS.Web.Application.ObjectItems;
using WIGOS.Web.Application.ObjectItems.Organisation;
using Microsoft.AspNetCore.Mvc;

namespace WIGOS.Web.API.Controllers.ObjectItems
{
    public class OrganisationsController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetOrganisations([FromQuery] PagingParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query
            {
                Category = ObjectItemCategory.Organisation,
                Params = param
            }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrganisation(Organisation organisation)
        {
            return HandleResult(await Mediator.Send(new Create.Command { Organisation = organisation }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrganisation(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }
    }
}