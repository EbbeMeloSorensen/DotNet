using DMI.StatDB.Persistence.Repositories;
using DMI.StatDB.Persistence.EntityFrameworkCore.SqlServer.Repositories;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.SqlServer
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
            Stations = new StationRepository(context);
            //PeopleAssociations = new PersonAssociationRepository(_context);
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
