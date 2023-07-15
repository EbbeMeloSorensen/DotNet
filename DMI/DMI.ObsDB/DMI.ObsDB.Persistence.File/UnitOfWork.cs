using DMI.ObsDB.Persistence.Repositories;

namespace DMI.ObsDB.Persistence.File
{
    public class UnitOfWork : IUnitOfWork
    {
        public IObservingFacilityRepository ObservingFacilities { get; }
        public ITimeSeriesRepository TimeSeries { get; }
        public IObservationRepository Observations { get; }

        public UnitOfWork(
            IObservingFacilityRepository observingFacilityRepository,
            ITimeSeriesRepository timeSeriesRepository,
            IObservationRepository observationRepository)
        {
            ObservingFacilities = observingFacilityRepository;
            TimeSeries = timeSeriesRepository;
            Observations = observationRepository;
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
