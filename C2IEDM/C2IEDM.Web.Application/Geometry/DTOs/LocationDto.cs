﻿namespace C2IEDM.Web.Application.Geometry.DTOs;

public class LocationDto
{
    public string type { get; set; }
    public Guid id { get; set; }

    public LocationDto()
    {
        type = "Location";
    }
}