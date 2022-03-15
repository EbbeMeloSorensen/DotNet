using Craft.Logging;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.SqlServer
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public Task<bool> CheckRepositoryConnection()
        {
            throw new NotImplementedException();
        }

        public IUnitOfWork GenerateUnitOfWork()
        {
            throw new NotImplementedException();
        }

        public void Initialize(ILogger logger)
        {
            throw new NotImplementedException();
        }
    }
}
