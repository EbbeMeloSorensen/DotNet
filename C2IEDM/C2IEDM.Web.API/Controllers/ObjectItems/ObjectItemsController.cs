using Microsoft.AspNetCore.Mvc;
using C2IEDM.Domain.Entities.ObjectItems;
using C2IEDM.Web.Application.ObjectItems;
using C2IEDM.Web.Application.ObjectItems.ObjectItem;
using C2IEDM.Domain.Entities.Geometry;

namespace C2IEDM.Web.API.Controllers.ObjectItems
{
    public class ObjectItemsController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetObjectItems([FromQuery] ObjectItemParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query
            {
                Category = ObjectItemCategory.ObjectItem,
                Params = param
            }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateObjectItem(ObjectItem objectItem)
        {
            return HandleResult(await Mediator.Send(new Create.Command { ObjectItem = objectItem }));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditObjectItem(Guid id, ObjectItem objectItem)
        {
            objectItem.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command { ObjectItem = objectItem }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteObjectItem(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }
    }
}
