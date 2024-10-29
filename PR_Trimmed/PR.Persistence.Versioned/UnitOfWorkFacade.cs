using System;
using PR.Persistence.Repositories;
using PR.Persistence.Versioned.Repositories;

namespace PR.Persistence.Versioned
{
    public class UnitOfWorkFacade : IUnitOfWork
    {
        private DateTime? _transactionTime;

        internal IUnitOfWork UnitOfWork { get; }
        internal DateTime? DatabaseTime { get; }
        internal DateTime? HistoricalTime { get; }

        internal DateTime TransactionTime => _transactionTime ??= DateTime.UtcNow;

        public IPersonRepository People { get; }

        public UnitOfWorkFacade(
            IUnitOfWork unitOfWork,
            DateTime? historicalTime,
            DateTime? databaseTime)
        {
            UnitOfWork = unitOfWork;
            HistoricalTime = historicalTime;
            DatabaseTime = databaseTime;

            People = new PersonRepositoryFacade(this);
        }

        public void Clear()
        {
            UnitOfWork.Clear();
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
