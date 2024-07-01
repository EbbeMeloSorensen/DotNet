using Craft.Logging;
using DMI.StatDB.Domain.Entities;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.Sqlite
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        static UnitOfWorkFactory()
        {
            using var context = new StatDBContext();
            context.Database.EnsureCreated();

            if (context.Stations.Any()) return;

            SeedDatabase(context);
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
            return new UnitOfWork(new StatDBContext());
        }

        private static void SeedDatabase(StatDBContext context)
        {
            var stations = new List<Station>
            {
                new Station
                {
                    Country = "Danmark",
                    StatID = 603000,
                    IcaoId = "",
                    Source = "ingres"
                },
                new Station
                {
                    Country = "Danmark",
                    StatID = 604200,
                    IcaoId = "",
                    Source = "ingres"
                }
            };

            context.Stations.AddRange(stations);
            context.SaveChanges();
        }
    }
}
