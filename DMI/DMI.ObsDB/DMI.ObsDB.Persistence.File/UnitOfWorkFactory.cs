using DMI.ObsDB.Persistence.File.Repositories;

namespace DMI.ObsDB.Persistence.File
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private ObservationRepository _observationRepository { get; }

        public UnitOfWorkFactory()
        {
            _observationRepository = new ObservationRepository();
        }

        public IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWork(_observationRepository);
        }
    }
}
