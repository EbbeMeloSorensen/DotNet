﻿namespace C2IEDM.Web.Application.Locations.DTOs;

public class AbsolutePointDto : PointDto
{
    public double latitudeCoordinate { get; set; }
    public double longitudeCoordinate { get; set; }

    public AbsolutePointDto()
    {
        type = "Absolute Point";
    }
}