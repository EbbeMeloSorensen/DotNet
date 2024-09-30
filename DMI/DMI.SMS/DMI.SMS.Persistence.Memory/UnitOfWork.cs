using DMI.SMS.Persistence.Repositories;

namespace DMI.SMS.Persistence.Memory
{
    public class UnitOfWork : IUnitOfWork
    {
        public IStationInformationRepository StationInformations { get; }
        public IImagesOfSensorLocationRepository ImagesOfSensorLocations { get; }
        public ISensorLocationRepository SensorLocations { get; }
        public ISensorInformationRepository SensorInformations { get; }
        public IContactPersonRepository ContactPersons { get; }
        public ILegalOwnerRepository LegalOwners { get; }
        public IElevationAnglesRepository ElevationAnglesRepository { get; }
        public IServiceVisitReportRepository ServiceVisitReportRepository { get; }

        public UnitOfWork(
            IStationInformationRepository stationInformationRepository)
        {
            StationInformations = stationInformationRepository;
        }

        public void Dispose()
        {
        }

        public int Complete()
        {
            return 0;
        }
    }
}
