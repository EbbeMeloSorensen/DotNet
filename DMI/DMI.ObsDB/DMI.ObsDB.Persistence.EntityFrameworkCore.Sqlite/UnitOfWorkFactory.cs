using Craft.Logging;
using DMI.ObsDB.Domain.Entities;

namespace DMI.ObsDB.Persistence.EntityFrameworkCore.Sqlite
{
    public class UnitOfWorkFactory : UnitOfWorkFactoryBase
    {
        static UnitOfWorkFactory()
        {
            using var context = new ObsDBContext();
            context.Database.EnsureCreated();

            if (context.ObservingFacilities.Any() ||
                context.TimeSeries.Any() ||
                context.Observations.Any()) return;

            SeedDatabase(context);
        }

        public override void Initialize(ILogger logger)
        {
        }

        public override Task<bool> CheckRepositoryConnection()
        {
            throw new NotImplementedException();
        }

        public override IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWork(new ObsDBContext());
        }

        private static void SeedDatabase(ObsDBContext context)
        {
            var observations = new List<Observation>
            {
                new Observation
                {
                    StatId = 601100,
                    ParamId = "temp_dry",
                    Time = new DateTime(1975, 7, 24, 7, 9, 0),
                    Value = 32.4
                },
                new Observation
                {
                    StatId = 601100,
                    ParamId = "temp_dry",
                    Time = new DateTime(1975, 7, 24, 7, 9, 15),
                    Value = 34.5
                },
                new Observation
                {
                    StatId = 603000,
                    ParamId = "temp_dry",
                    Time = new DateTime(1975, 7, 24, 7, 9, 0),
                    Value = 34.4
                },
                new Observation
                {
                    StatId = 603000,
                    ParamId = "temp_dry",
                    Time = new DateTime(1975, 7, 24, 7, 9, 15),
                    Value = 32.5
                },
                new Observation
                {
                    StatId = 601100,
                    ParamId = "wind_speed",
                    Time = new DateTime(1975, 7, 24, 7, 9, 0),
                    Value = 7.2
                },
                new Observation
                {
                    StatId = 601100,
                    ParamId = "wind_speed",
                    Time = new DateTime(1975, 7, 24, 7, 9, 15),
                    Value = 7.4
                },
            };

            context.Observations.AddRange(observations);
            context.SaveChanges();
        }
    }
}
