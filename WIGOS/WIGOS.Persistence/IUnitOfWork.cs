using WIGOS.Persistence.Repositories;
using WIGOS.Persistence.Repositories.Geometry;
using WIGOS.Persistence.Repositories.WIGOS;

namespace WIGOS.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IPersonRepository People { get; }

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
        Repositories.Geometry.IPointRepository Points { get; }
        IPolyArcAreaRepository PolyArcAreas { get; }
        IPolygonAreaRepository PolygonAreas { get; }
        IRelativePointRepository RelativePoints { get; }
        ISphereVolumeRepository SphereVolumes { get; }
        ISurfaceRepository Surfaces { get; }
        ISurfaceVolumeRepository SurfaceVolumes { get; }
        ITrackAreaRepository TrackAreas { get; }
        IVerticalDistanceRepository VerticalDistances { get; }

        IAbstractEnvironmentalMonitoringFacilityRepository AbstractEnvironmentalMonitoringFacilities { get; }
        IObservingFacilityRepository ObservingFacilities { get; }
        IGeospatialLocationRepository GeospatialLocations { get; }
        Repositories.WIGOS.IPointRepository Points_Wigos { get; }

        int Complete();
    }
}