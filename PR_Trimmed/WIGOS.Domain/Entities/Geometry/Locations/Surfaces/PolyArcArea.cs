using WIGOS.Domain.Entities.Geometry.Locations.Points;

namespace WIGOS.Domain.Entities.Geometry.Locations.Surfaces
{
    public class PolyArcArea : Surface
    {
        public Guid DefiningLineId { get; set; }
        public Line.Line DefiningLine { get; set; } = null!;

        public Guid BearingOriginPointId { get; set; }
        public Point BearingOriginPoint { get; set; } = null!;

        public double BeginBearingAngle { get; set; }
        public double EndBearingAngle { get; set; }
        public double ArcRadiusDimension { get; set; }

        public PolyArcArea(
            Guid objectId,
            DateTime created) : base(objectId, created)
        {
        }
    }
}