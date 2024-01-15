using WIGOS.Domain.Entities.ObjectItems.Organisations;
using WIGOS.Web.Application.Core;
using WIGOS.Web.Application.ObjectItems;
using WIGOS.Web.Application.ObjectItems.Unit;
using Microsoft.AspNetCore.Mvc;

namespace WIGOS.Web.API.Controllers.ObjectItems
{
    public class UnitsController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetUnits([FromQuery] PagingParams param)
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
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }
    }
}