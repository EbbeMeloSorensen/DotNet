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

        public UnitOfWork(
            C2IEDMDbContextBase context)
        {
            _context = context;
            Locations = new LocationRepository(_context);
            Points = new PointRepository(_context);
            AbsolutePoints = new AbsolutePointRepository(_context);
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
