using C2IEDM.Persistence.EntityFrameworkCore.Repositories;
using C2IEDM.Persistence.Repositories.Geometry;
using C2IEDM.Persistence.EntityFrameworkCore.Repositories.Geometry;
using C2IEDM.Persistence.Repositories;

namespace C2IEDM.Persistence.EntityFrameworkCore
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly C2IEDMDbContextBase _context;

        public IPersonRepository People { get; }

        public IAbsolutePointRepository AbsolutePoints { get; }
        public IConeVolumeRepository ConeVolumes { get; }
        public ICoordinateSystemRepository CoordinateSystems { get; }
        public ICorridorAreaRepository CorridorAreas { get; }
        public IEllipseRepository Ellipses { get; }
        public IFanAreaRepository FanAreas { get; }
        public IGeometricVolumeRepository GeometricVolumes { get; }
        public ILinePointRepository LinePoints { get; }
        public ILineRepository Lines { get; }
        public ILocationRepository Locations { get; }
        public IOrbitAreaRepository OrbitAreas { get; }
        public IPointReferenceRepository PointReferences { get; }
        public IPointRepository Points { get; }
        public IPolyArcAreaRepository PolyArcAreas { get; }
        public IPolygonAreaRepository PolygonAreas { get; }
        public IRelativePointRepository RelativePoints { get; }
        public ISphereVolumeRepository SphereVolumes { get; }
        public ISurfaceRepository Surfaces { get; }
        public ISurfaceVolumeRepository SurfaceVolumes { get; }
        public ITrackAreaRepository TrackAreas { get; }
        public IVerticalDistanceRepository VerticalDistances { get; }

        public UnitOfWork(
            C2IEDMDbContextBase context)
        {
            _context = context;

            People = new PersonRepository(_context);

            AbsolutePoints = new AbsolutePointRepository(_context);
            ConeVolumes = new ConeVolumeRepository(_context);
            CoordinateSystems = new CoordinateSystemRepository(_context);
            CorridorAreas = new CorridorAreaRepository(_context);
            Ellipses = new EllipseRepository(_context);
            FanAreas = new FanAreaRepository(_context);
            GeometricVolumes = new GeometricVolumeRepository(_context);
            LinePoints = new LinePointRepository(_context);
            Lines = new LineRepository(_context);
            Locations = new LocationRepository(_context);
            OrbitAreas = new OrbitAreaRepository(_context);
            PointReferences = new PointReferenceRepository(_context);
            Points = new PointRepository(_context);
            PolyArcAreas = new PolyArcAreaRepository(_context);
            PolygonAreas = new PolygonAreaRepository(_context);
            RelativePoints = new RelativePointRepository(_context);
            SphereVolumes = new SphereVolumeRepository(_context);
            Surfaces = new SurfaceRepository(_context);
            SurfaceVolumes = new SurfaceVolumeRepository(_context);
            TrackAreas = new TrackAreaRepository(_context);
            VerticalDistances = new VerticalDistanceRepository(_context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
