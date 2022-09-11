using Craft.Logging;

namespace PR.Persistence.EntityFrameworkCore.SqlServer
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        static UnitOfWorkFactory()
        {
            var dbContext = new PRDbContext();
            dbContext.Database.EnsureCreated();
        }

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
