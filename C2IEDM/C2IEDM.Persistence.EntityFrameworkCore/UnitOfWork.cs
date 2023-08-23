using C2IEDM.Persistence.Repositories.Geometry;
using C2IEDM.Persistence.EntityFrameworkCore.Repositories.Geometry;

namespace C2IEDM.Persistence.EntityFrameworkCore
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly C2IEDMDbContextBase _context;

        public ILocationRepository Locations { get; }
        public IPointRepository Points { get; }
        public IAbsolutePointRepository AbsolutePoints { get; }
        public IVerticalDistanceRepository VerticalDistances { get; }
        public ILineRepository Lines { get; }
        public ILinePointRepository LinePoints { get; }
        public IEllipseRepository Ellipses { get; }
        public ICorridorAreaRepository CorridorAreas { get; }
        public IPolygonAreaRepository PolygonAreas { get; }
        public IFanAreaRepository FanAreas { get; }

        public UnitOfWork(
            C2IEDMDbContextBase context)
        {
            _context = context;
            Locations = new LocationRepository(_context);
            Points = new PointRepository(_context);
            AbsolutePoints = new AbsolutePointRepository(_context);
            VerticalDistances = new VerticalDistanceRepository(_context);
            Lines = new LineRepository(_context);
            LinePoints = new LinePointRepository(_context);
            Ellipses = new EllipseRepository(_context);
            CorridorAreas = new CorridorAreaRepository(_context);
            PolygonAreas = new PolygonAreaRepository(_context);
            FanAreas = new FanAreaRepository(_context);
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
