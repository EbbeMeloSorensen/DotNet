﻿using C2IEDM.Web.Application.Locations;
using Microsoft.AspNetCore.Mvc;

namespace C2IEDM.Web.API.Controllers.Location;

public class PolygonAreasController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetPolygonAreas([FromQuery] LocationParams param)
    {
        return HandlePagedResult(await Mediator.Send(new ListLocations.Query
        {
            Type = LocationType.PolygonArea,
            Params = param
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPolygonArea(Guid id)
    {
        return HandleResult(await Mediator.Send(new PolygonAreaDetails.Query { Id = id }));
    }
}