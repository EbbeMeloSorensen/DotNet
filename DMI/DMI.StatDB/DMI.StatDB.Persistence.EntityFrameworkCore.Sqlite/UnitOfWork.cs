using DMI.StatDB.Persistence.Repositories;
using DMI.StatDB.Persistence.EntityFrameworkCore.Sqlite.Repositories;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.Sqlite
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StatDBContext _context;

        public IStationRepository Stations { get; }
        public IPositionRepository Positions { get; }

        public UnitOfWork(StatDBContext context)
        {
            _context = context;
            Stations = new StationRepository(_context);
            Positions = new PositionRepository(_context);
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
