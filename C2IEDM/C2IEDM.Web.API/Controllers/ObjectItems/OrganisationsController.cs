using Microsoft.AspNetCore.Mvc;
using C2IEDM.Domain.Entities.ObjectItems.Organisations;
using C2IEDM.Web.Application.ObjectItems;
using C2IEDM.Web.Application.ObjectItems.Organisation;

namespace C2IEDM.Web.API.Controllers.ObjectItems;

public class OrganisationsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetOrganisations([FromQuery] ObjectItemParams param)
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
        return HandleResult(await Mediator.Send(new Application.ObjectItems.ObjectItem.Delete.Command { Id = id }));
    }
}