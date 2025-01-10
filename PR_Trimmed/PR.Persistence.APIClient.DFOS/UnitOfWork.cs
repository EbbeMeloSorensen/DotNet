using PR.Persistence.APIClient.DFOS.Repositories;
using PR.Persistence.Repositories;
using System;
using Craft.Logging;

namespace PR.Persistence.APIClient.DFOS
{
    public class UnitOfWork : IUnitOfWork
    {
        public IPersonRepository People { get; }
        public IPersonCommentRepository PersonComments { get; }


        public UnitOfWork(
            ILogger logger,
            string baseURL,
            DateTime? historicalTime,
            DateTime? databaseTime)
        {
            People = new PersonRepository(
                logger,
                baseURL,
                historicalTime,
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
