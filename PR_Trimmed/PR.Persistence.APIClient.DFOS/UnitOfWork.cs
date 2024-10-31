using PR.Persistence.APIClient.DFOS.Repositories;
using PR.Persistence.Repositories;
using System;

namespace PR.Persistence.APIClient.DFOS
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
        }

        public void Dispose()
        {
        }
    }
}
