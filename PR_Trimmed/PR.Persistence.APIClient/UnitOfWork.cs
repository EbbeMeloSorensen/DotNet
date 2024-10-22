using System;
using PR.Persistence.APIClient.Repositories;
using PR.Persistence.Repositories;

namespace PR.Persistence.APIClient
{
    public class UnitOfWork : IUnitOfWork
    {
        public IPersonRepository People { get; }

        public UnitOfWork(
            DateTime? databaseTime)
        {
            People = new PersonRepository(databaseTime);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Complete()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
