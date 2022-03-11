using System;
using System.Threading.Tasks;
using Craft.Logging;

namespace DMI.StatDB.Persistence.EntityFramework
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        static UnitOfWorkFactory()
        {
            using (var dbContext = new StatDbContext())
            {
                dbContext.Database.CreateIfNotExists();
            }
        }

        public void Initialize(
            ILogger logger)
        {
        }

        public async Task<bool> CheckRepositoryConnection()
        {
            return await Task.Run(() =>
            {
                using (var dbContext = new StatDbContext())
                {
                    try
                    {
                        dbContext.Database.Connection.Open();
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            });
        }

        public IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWork(new StatDbContext());
        }
    }
}
