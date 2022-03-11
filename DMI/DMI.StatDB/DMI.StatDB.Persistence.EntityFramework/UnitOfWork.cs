using System;
using DMI.StatDB.Persistence.EntityFramework.Repositories;
using DMI.StatDB.Persistence.Repositories;

namespace DMI.StatDB.Persistence.EntityFramework
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StatDbContext _context;

        public IStationRepository Stations { get; }
        public IPositionRepository Positions { get; }

        public UnitOfWork(
            StatDbContext context)
        {
            _context = context;

            Stations = new StationRepository(_context);
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
