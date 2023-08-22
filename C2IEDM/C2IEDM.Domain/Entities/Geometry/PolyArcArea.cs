namespace C2IEDM.Domain.Entities.Geometry
{
    public class PolyArcArea : Surface
    {
        public Guid DefiningLineId { get; set; }
        public Line DefiningLine { get; set; } = null!;

        public Guid BearingOriginPointId { get; set; }
        public Point BearingOriginPoint { get; set; } = null!;

        public double BeginBearingAngle { get; set; }
        public double EndBearingAngle { get; set; }
        public double ArcRadiusDimension { get; set; }
    }
}