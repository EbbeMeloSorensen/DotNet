namespace C2IEDM.Domain.Entities.Geometry
{
    public class ConeVolume : GeometricVolume
    {
        public Guid DefiningSurfaceId { get; set; }
        public Surface DefiningSurface { get; set; } = null!;

        public Guid VertexPointId { get; set; }
        public Point VertexPoint { get; set; } = null!;
    }
}