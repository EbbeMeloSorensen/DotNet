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
            var station1 = new Station
            {
                Country = "Danmark",
                StatID = 603000,
                IcaoId = "",
                Source = "ingres"
            };

            var station2 = new Station
            {
                Country = "Danmark",
                StatID = 604200,
                IcaoId = "",
                Source = "ingres"
            };

            var stations = new List<Station>
            {
                station1,
                station2
            };

            var positions = new List<Position>
            {
                new Position
                {
                    Lat = 9.12,
                    Long = 56.34,
                    StartTime = new DateTime(2012, 7, 24, 13, 0, 0),
                    EndTime = new DateTime(2019, 7, 24, 13, 0, 0),
                    Station = station1,
                    Entity = "station"
                },
                new Position
                {
                    Lat = 9.12,
                    Long = 55.34,
                    StartTime = new DateTime(2019, 7, 24, 13, 0, 0),
                    EndTime = new DateTime(2023, 7, 24, 13, 0, 0),
                    Station = station1,
                    Entity = "station"
                },
                new Position
                {
                    Lat = 9.12,
                    Long = 56.34,
                    StartTime = new DateTime(2011, 7, 24, 13, 0, 0),
                    EndTime = new DateTime(2016, 7, 24, 13, 0, 0),
                    Station = station2,
                    Entity = "station"
                },
                new Position
                {
                    Lat = 9.12,
                    Long = 55.34,
                    StartTime = new DateTime(2016, 7, 24, 13, 0, 0),
                    EndTime = new DateTime(2023, 7, 24, 13, 0, 0),
                    Station = station2,
                    Entity = "station"
                }
            };

            context.Stations.AddRange(stations);
            context.Positions.AddRange(positions);
            context.SaveChanges();
        }
    }
}
