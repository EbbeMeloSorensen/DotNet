﻿using Microsoft.AspNetCore.Mvc;
using C2IEDM.Web.Application.Locations;

namespace C2IEDM.Web.API.Controllers.Location;

public class LocationsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetLocations([FromQuery] LocationParams param)
    {
        return HandlePagedResult(await Mediator.Send(new ListLocations.Query
        {
            Type = LocationType.Location,
            Params = param
        }));
    }
}