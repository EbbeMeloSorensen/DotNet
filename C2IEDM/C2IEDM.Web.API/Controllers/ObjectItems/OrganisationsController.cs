﻿using C2IEDM.Web.Application.ObjectItems;
using Microsoft.AspNetCore.Mvc;

namespace C2IEDM.Web.API.Controllers.ObjectItems;

public class OrganisationsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetOrganisations([FromQuery] ObjectItemParams param)
    {
        return HandlePagedResult(await Mediator.Send(new ListObjectItems.Query
        {
            Category = ObjectItemCategory.Organisation,
            Params = param
        }));
    }
}