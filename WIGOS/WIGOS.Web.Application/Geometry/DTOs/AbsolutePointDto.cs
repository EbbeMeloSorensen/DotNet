using WIGOS.Web.Application.Geometry.VerticalDistance;

namespace WIGOS.Web.Application.Geometry.DTOs
{
    public class AbsolutePointDto : PointDto
    {
        public double latitudeCoordinate { get; set; }
        public double longitudeCoordinate { get; set; }

        public VerticalDistanceDto VerticalDistance { get; set; }

        public AbsolutePointDto()
        {
            type = "Absolute Point";
        }
    }
}