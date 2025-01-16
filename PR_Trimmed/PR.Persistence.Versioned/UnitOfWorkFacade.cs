using System;
using Craft.Logging;
using PR.Persistence.Repositories.PR;
using PR.Persistence.Repositories.Smurfs;
using PR.Persistence.Versioned.Repositories;

namespace PR.Persistence.Versioned
{
    public class UnitOfWorkFacade : IUnitOfWork
    {
        private DateTime? _transactionTime;

        internal IUnitOfWork UnitOfWork { get; }
        internal DateTime? DatabaseTime { get; }
        internal DateTime? HistoricalTime { get; }
        internal bool IncludeCurrentObjects { get; }
        internal bool IncludeHistoricalObjects { get; }

        internal DateTime TransactionTime => _transactionTime ??= DateTime.UtcNow;

        public ISmurfRepository Smurfs { get; }

        public IPersonRepository People { get; }
        public IPersonCommentRepository PersonComments { get; }

        public UnitOfWorkFacade(
            ILogger logger,
            IUnitOfWork unitOfWork,
            DateTime? historicalTime,
            DateTime? databaseTime,
            bool includeCurrentObjects,
            bool includeHistoricalObjects)
        {
            UnitOfWork = unitOfWork;
            HistoricalTime = historicalTime;
            DatabaseTime = databaseTime;
            IncludeCurrentObjects = includeCurrentObjects;
            IncludeHistoricalObjects = includeHistoricalObjects;

            People = new PersonRepositoryFacade(logger, this);
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
