using System;
using PR.Persistence.Repositories;
using PR.Persistence.Versioned.Repositories;

namespace PR.Persistence.Versioned
{
    public class UnitOfWorkFacade : IUnitOfWork
    {
        private DateTime? _transactionTime;

        public IUnitOfWork UnitOfWork { get; }
        public DateTime? DatabaseTime { get; }

        public DateTime TransactionTime => _transactionTime ??= DateTime.UtcNow;

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
