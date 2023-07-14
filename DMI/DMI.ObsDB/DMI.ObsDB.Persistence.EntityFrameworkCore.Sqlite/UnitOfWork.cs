using DMI.ObsDB.Persistence.EntityFrameworkCore.Sqlite.Repositories;
using DMI.ObsDB.Persistence.Repositories;

namespace DMI.ObsDB.Persistence.EntityFrameworkCore.Sqlite
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ObsDBContext _context;

        public IObservingFacilityRepository ObservingFacilities { get; }
        public ITimeSeriesRepository TimeSeries { get; }
        public IObservationRepository Observations { get; }

        public UnitOfWork(ObsDBContext context)
        {
            _context = context;
            ObservingFacilities = new ObservingFacilityRepository(_context);
            TimeSeries = new TimeSeriesRepository(_context);
            Observations = new ObservationRepository(_context);
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
