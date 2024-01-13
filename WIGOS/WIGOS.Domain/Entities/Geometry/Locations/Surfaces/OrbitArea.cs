using WIGOS.Domain.Entities.Geometry.Locations.Points;

namespace WIGOS.Domain.Entities.Geometry.Locations.Surfaces
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

        public OrbitArea(
            Guid objectId,
            DateTime created) : base(objectId, created)
        {
        }
    }
}