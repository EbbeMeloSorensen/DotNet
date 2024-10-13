using System;
using PR.Persistence.Repositories;
using PR.Persistence.RepositoryFacades;

namespace PR.Persistence
{
    public class UnitOfWorkFacade : IUnitOfWork
    {
        private DateTime? _transactionTime;

        public IUnitOfWork UnitOfWork { get; }
        public DateTime? DatabaseTime { get; }

        internal DateTime TransactionTime => _transactionTime ??= DateTime.UtcNow;

        public IPersonRepository People { get; }

        public UnitOfWorkFacade(
            IUnitOfWork unitOfWork,
            DateTime? databaseTime)
        {
            UnitOfWork = unitOfWork;
            DatabaseTime = databaseTime;

            People = new PersonRepositoryFacade(this);
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