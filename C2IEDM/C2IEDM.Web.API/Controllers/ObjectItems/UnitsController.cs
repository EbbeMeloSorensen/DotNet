using Microsoft.AspNetCore.Mvc;
using C2IEDM.Domain.Entities.ObjectItems.Organisations;
using C2IEDM.Web.Application.ObjectItems;
using C2IEDM.Web.Application.ObjectItems.Unit;

namespace C2IEDM.Web.API.Controllers.ObjectItems;

public class UnitsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetUnits([FromQuery] ObjectItemParams param)
    {
        return HandlePagedResult(await Mediator.Send(new List.Query
        {
            Category = ObjectItemCategory.Unit,
            Params = param
        }));
    }

    [HttpPost]
    public async Task<IActionResult> CreateUnit(Unit unit)
    {
        return HandleResult(await Mediator.Send(new Create.Command { Unit = unit }));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUnit(Guid id)
    {
        return HandleResult(await Mediator.Send(new Application.ObjectItems.ObjectItem.Delete.Command { Id = id }));
    }
}