using DMI.StatDB.Persistence.Repositories;

namespace DMI.StatDB.Persistence.File
{
    public class UnitOfWork : IUnitOfWork
    {
        public IStationRepository Stations { get; }

        public UnitOfWork(
            IStationRepository stationRepository)
        {
            Stations = stationRepository;
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
