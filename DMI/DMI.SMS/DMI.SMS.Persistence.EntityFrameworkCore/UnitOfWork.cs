using DMI.SMS.Persistence.Repositories;
using DMI.SMS.Persistence.EntityFrameworkCore.Repositories;

namespace DMI.SMS.Persistence.EntityFrameworkCore
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SMSDbContextBase _context;

        public IStationInformationRepository StationInformations { get; }
        public IContactPersonRepository ContactPersons { get; }
        public ILegalOwnerRepository LegalOwners { get; }
        public ISensorLocationRepository SensorLocations { get; }
        public ISensorInformationRepository SensorInformations { get; }
        public IImagesOfSensorLocationRepository ImagesOfSensorLocations { get; }
        public IElevationAnglesRepository ElevationAnglesRepository { get; }
        public IServiceVisitReportRepository ServiceVisitReportRepository { get; }

        public UnitOfWork(SMSDbContextBase context)
        {
            _context = context;
            StationInformations = new StationInformationRepository(_context);
            SensorLocations = new SensorLocationRepository(_context);
            ElevationAnglesRepository = new ElevationAnglesRepository(_context);
            ServiceVisitReportRepository = new ServiceVisitReportRepository(_context);
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
