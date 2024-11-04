using System;
using PR.Persistence.APIClient.Repositories;
using PR.Persistence.Repositories;

namespace PR.Persistence.APIClient
{
    public class UnitOfWork : IUnitOfWork
    {
        public IPersonRepository People { get; }

        public UnitOfWork(
            DateTime? historicalTime,
            bool includeHistoricalObjects,
            DateTime? databaseTime)
        {
            People = new PersonRepository(
                historicalTime,
                includeHistoricalObjects,
                databaseTime);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Complete()
        {
        }

        public void Dispose()
        {
        }
    }
}
