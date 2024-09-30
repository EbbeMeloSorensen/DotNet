using System;
using DMI.SMS.Persistence.Repositories;

namespace DMI.SMS.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IStationInformationRepository StationInformations { get; }
        IContactPersonRepository ContactPersons { get; }
        ILegalOwnerRepository LegalOwners { get; }
        ISensorLocationRepository SensorLocations { get; }
        ISensorInformationRepository SensorInformations { get; }
        IImagesOfSensorLocationRepository ImagesOfSensorLocations { get; }
        IElevationAnglesRepository ElevationAnglesRepository { get; }
        IServiceVisitReportRepository ServiceVisitReportRepository { get; }

        int Complete();
    }
}
