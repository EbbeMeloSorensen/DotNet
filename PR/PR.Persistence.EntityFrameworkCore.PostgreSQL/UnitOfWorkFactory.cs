using Craft.Logging;

namespace PR.Persistence.EntityFrameworkCore.PostgreSQL
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
            return new UnitOfWork(new PRDbContext());
        }
    }
}