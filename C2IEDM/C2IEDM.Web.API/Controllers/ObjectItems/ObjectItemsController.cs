using C2IEDM.Domain.Entities.ObjectItems;
using Microsoft.AspNetCore.Mvc;
using C2IEDM.Web.Application.ObjectItems;
using C2IEDM.Web.Application.Locations;

namespace C2IEDM.Web.API.Controllers.ObjectItems
{
    public class ObjectItemsController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetObjectItems([FromQuery] ObjectItemParams param)
        {
            return HandlePagedResult(await Mediator.Send(new ListObjectItems.Query
            {
                Category = ObjectItemCategory.ObjectItem,
                Params = param
            }));
        }
    }
}
