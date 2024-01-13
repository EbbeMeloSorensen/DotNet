using WIGOS.Domain.Entities.Geometry.Locations.Points;
using WIGOS.Domain.Entities.Geometry.Locations.Surfaces;

namespace WIGOS.Domain.Entities.Geometry.Locations.GeometricVolumes
{
    public class ConeVolume : GeometricVolume
    {
        public Guid DefiningSurfaceId { get; set; }
        public Surface DefiningSurface { get; set; } = null!;

        public Guid VertexPointId { get; set; }
        public Point VertexPoint { get; set; } = null!;

        public ConeVolume(
            Guid objectId,
            DateTime created) : base(objectId, created)
        {
        }
    }
}