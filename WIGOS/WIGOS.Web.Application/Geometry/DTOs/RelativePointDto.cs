﻿namespace WIGOS.Web.Application.Geometry.DTOs
{
    public class RelativePointDto : PointDto
    {
        public CoordinateSystemDto CoordinateSystem { get; set; }

        public double XCoordinateDimension { get; set; }
        public double YCoordinateDimension { get; set; }
        public double ZCoordinateDimension { get; set; }

        public RelativePointDto()
        {
            type = "Relative Point";
        }
    }
}