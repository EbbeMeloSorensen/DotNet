using System;
using PR.Persistence.RepositoryFacades;

namespace PR.Persistence
{
    public class UnitOfWorkFacade : IDisposable
    {
        private DateTime? _transactionTime;

        public IUnitOfWork UnitOfWork { get; }
        public DateTime? DatabaseTime { get; }

        internal DateTime TransactionTime => _transactionTime ??= DateTime.UtcNow;

        public PersonRepositoryFacade People { get; }
        public PersonAssociationRepositoryFacade PersonAssociations { get; }

        public UnitOfWorkFacade(
            IUnitOfWork unitOfWork,
            DateTime? databaseTime)
        {
            UnitOfWork = unitOfWork;
            DatabaseTime = databaseTime;

            People = new PersonRepositoryFacade(this);
            PersonAssociations = new PersonAssociationRepositoryFacade(this);
        }

        public void Complete()
        {
            UnitOfWork.Complete();
            _transactionTime = null;
        }

        public void Dispose()
        {
            UnitOfWork.Dispose();
        }
    }
}