using System;
using System.Threading.Tasks;
using Craft.Logging;

namespace DD.Persistence.Npgsql
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public void Initialize(ILogger logger)
        {
        }

        public Task<bool> CheckRepositoryConnection()
        {
            throw new NotImplementedException();
        }

        public IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWork();
        }
    }
}
