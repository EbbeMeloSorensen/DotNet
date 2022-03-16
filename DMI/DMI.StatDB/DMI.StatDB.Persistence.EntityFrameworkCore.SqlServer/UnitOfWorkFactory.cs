using Craft.Logging;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.SqlServer
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        static UnitOfWorkFactory()
        {
            using (var dbContext = new StatDbContext())
            {
                //dbContext.Database.CreateIfNotExists();
                dbContext.Database.EnsureCreated();
            }
        }

        public void Initialize(
            ILogger logger)
        {
        }

        public Task<bool> CheckRepositoryConnection()
        {
            throw new NotImplementedException();
        }

        public IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWork(new StatDbContext());
        }
    }
}
