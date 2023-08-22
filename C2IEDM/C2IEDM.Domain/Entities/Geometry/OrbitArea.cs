namespace C2IEDM.Domain.Entities.Geometry
{
    public enum OrbitAreaAlignmentCode
    {
        Centre,
        Left,
        Right
    }

    public class OrbitArea : Surface
    {
        public Guid FirstPointId { get; set; }
        public Point FirstPoint { get; set; } = null!;

        public Guid SecondPointId { get; set; }
        public Point SecondPoint { get; set; } = null!;

        public OrbitAreaAlignmentCode OrbitAreaAlignmentCode { get; set; }
        public double WidthDimension { get; set; }
    }
}