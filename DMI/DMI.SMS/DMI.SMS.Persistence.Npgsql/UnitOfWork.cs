using System.Transactions;
using DMI.SMS.Persistence.Npgsql.Repositories;
using DMI.SMS.Persistence.Repositories;

namespace DMI.SMS.Persistence.Npgsql
{
    public class UnitOfWork : IUnitOfWork
    {
        private TransactionScope _scope;

        public IStationInformationRepository StationInformations { get; }
        public IImagesOfSensorLocationRepository ImagesOfSensorLocations { get; }
        public ISensorLocationRepository SensorLocations { get; }
        public ISensorInformationRepository SensorInformations { get; }
        public IContactPersonRepository ContactPersons { get; }
        public ILegalOwnerRepository LegalOwners { get; }
        public IStationKeeperRepository StationKeepers { get; }
        public IElevationAnglesRepository ElevationAnglesRepository { get; }
        public IServiceVisitReportRepository ServiceVisitReportRepository { get; }

        public UnitOfWork()
        {
            try
            {
                _scope = new TransactionScope();
                StationInformations = new StationInformationRepository();
                ContactPersons = new ContactPersonRepository();
                LegalOwners = new LegalOwnerRepository();
                StationKeepers = new StationKeeperRepository();
            }
            catch(Exception e)
            {
                _scope.Dispose();
                throw e;
            }
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        public int Complete()
        {
            _scope.Complete();
            return 0;
        }
    }
}
