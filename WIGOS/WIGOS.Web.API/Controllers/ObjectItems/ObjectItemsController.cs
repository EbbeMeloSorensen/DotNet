using WIGOS.Domain.Entities.ObjectItems;
using WIGOS.Web.Application.Core;
using WIGOS.Web.Application.ObjectItems;
using WIGOS.Web.Application.ObjectItems.ObjectItem;
using Microsoft.AspNetCore.Mvc;

namespace WIGOS.Web.API.Controllers.ObjectItems
{
    public class ObjectItemsController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetObjectItems([FromQuery] PagingParams param)
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
