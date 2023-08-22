namespace C2IEDM.Domain.Entities.Geometry
{
    public class CorridorArea : Surface
    {
        public Guid CenterLineId { get; set; }
        public Line CenterLine { get; set; } = null!;

        public double WidthDimension { get; set; }
    }
}