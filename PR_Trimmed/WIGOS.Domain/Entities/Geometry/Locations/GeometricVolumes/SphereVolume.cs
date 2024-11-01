using WIGOS.Domain.Entities.Geometry.Locations.Points;

namespace WIGOS.Domain.Entities.Geometry.Locations.GeometricVolumes
{
    public class SphereVolume : GeometricVolume
    {
        public Guid CentrePointId { get; set; }
        public Point CentrePoint { get; set; } = null!;

        public double RadiusDimension { get; set; }

        public SphereVolume(
            Guid objectId,
            DateTime created) : base(objectId, created)
        {
        }
    }
}