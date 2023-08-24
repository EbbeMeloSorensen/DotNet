using C2IEDM.Persistence.Repositories.Geometry;

namespace C2IEDM.Persistence;

public interface IUnitOfWork : IDisposable
{
    IAbsolutePointRepository AbsolutePoints { get; }
    IConeVolumeRepository ConeVolumes { get; }
    ICoordinateSystemRepository CoordinateSystems { get; }
    ICorridorAreaRepository CorridorAreas { get; }
    IEllipseRepository Ellipses { get; }
    IFanAreaRepository FanAreas { get; }
    IGeometricVolumeRepository GeometricVolumes { get; }
    ILinePointRepository LinePoints { get; }
    ILineRepository Lines { get; }
    ILocationRepository Locations { get; }
    IOrbitAreaRepository OrbitAreas { get; }
    IPointReferenceRepository PointReferences { get; }
    IPointRepository Points { get; }
    IPolyArcAreaRepository PolyArcAreas { get; }
    IPolygonAreaRepository PolygonAreas { get; }
    IRelativePointRepository RelativePoints { get; }
    ISphereVolumeRepository SphereVolumes { get; }
    ISurfaceRepository Surfaces { get; }
    ISurfaceVolumeRepository SurfaceVolumes { get; }
    ITrackAreaRepository TrackAreas { get; }
    IVerticalDistanceRepository VerticalDistances { get; }

    int Complete();
}