using DMI.StatDB.Persistence.Repositories;

namespace DMI.StatDB.Persistence.File
{
    public class UnitOfWork : IUnitOfWork
    {
        public IStationRepository Stations { get; }
        public IPositionRepository Positions { get; }

        public UnitOfWork(
            IStationRepository stationRepository,
            IPositionRepository positionRepository)
        {
            Stations = stationRepository;
            Positions = positionRepository;
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
