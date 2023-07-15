using DMI.ObsDB.Persistence.File.Repositories;

namespace DMI.ObsDB.Persistence.File
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private ObservingFacilityRepository _observingFacilityRepository { get; }
        private TimeSeriesRepository _timeSeriesRepository { get; }
        private ObservationRepository _observationRepository { get; }

        public UnitOfWorkFactory()
        {
            _observingFacilityRepository = new ObservingFacilityRepository();
            _timeSeriesRepository = new TimeSeriesRepository();
            _observationRepository = new ObservationRepository();
        }

        public IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWork(
                _observingFacilityRepository,
                _timeSeriesRepository,
                _observationRepository);
        }
    }
}
