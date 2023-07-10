using DMI.ObsDB.Persistence.Repositories;

namespace DMI.ObsDB.Persistence.File
{
    public class UnitOfWork : IUnitOfWork
    {
        public IObservationRepository Observations { get; }

        public UnitOfWork(
            IObservationRepository observationRepository)
        {
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
