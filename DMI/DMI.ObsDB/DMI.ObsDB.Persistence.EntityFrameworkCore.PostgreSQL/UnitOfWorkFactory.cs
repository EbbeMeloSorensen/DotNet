using Craft.Logging;
using DMI.ObsDB.Domain.Entities;

namespace DMI.ObsDB.Persistence.EntityFrameworkCore.PostgreSQL
{
    public class UnitOfWorkFactory : UnitOfWorkFactoryBase
    {
        static UnitOfWorkFactory()
        {
            using var context = new ObsDBContext();

            try
            {
                context.Database.EnsureCreated();
            }
            catch (System.Exception e)
            {
                throw;
            }

            if (context.ObservingFacilities.Any() ||
                context.TimeSeries.Any() ||
                context.Observations.Any()) return;

            //SeedDatabase(context);
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
            var observingFacility1 = new ObservingFacility
            {
                StatId = 601100
            };

            var observingFacility2 = new ObservingFacility
            {
                StatId = 603000
            };

            var observingFacilities = new List<ObservingFacility>
            {
                observingFacility1,
                observingFacility2,
            };

            var timeSeries1 = new TimeSeries
            {
                ParamId = "temp_dry",
                ObservingFacility = observingFacility1
            };

            var timeSeries2 = new TimeSeries
            {
                ParamId = "wind_speed",
                ObservingFacility = observingFacility1
            };

            var timeSeries3 = new TimeSeries
            {
                ParamId = "temp_dry",
                ObservingFacility = observingFacility2
            };

            var timeSeries = new List<TimeSeries>
            {
                timeSeries1,
                timeSeries2,
                timeSeries3,
            };

            var observations = new List<Observation>
            {
                new Observation
                {
                    TimeSeries = timeSeries1,
                    Time = new DateTime(1975, 7, 24, 7, 9, 0),
                    Value = 32.4
                },
                new Observation
                {
                    TimeSeries = timeSeries1,
                    Time = new DateTime(1975, 7, 24, 7, 9, 15),
                    Value = 34.5
                },
                new Observation
                {
                    TimeSeries = timeSeries1,
                    Time = new DateTime(1975, 7, 24, 7, 9, 30),
                    Value = 36.1
                },
                new Observation
                {
                    TimeSeries = timeSeries2,
                    Time = new DateTime(1975, 7, 24, 7, 9, 0),
                    Value = 7.2
                },
                new Observation
                {
                    TimeSeries = timeSeries2,
                    Time = new DateTime(1975, 7, 24, 7, 9, 15),
                    Value = 7.4
                },
                new Observation
                {
                    TimeSeries = timeSeries3,
                    Time = new DateTime(1990, 7, 24, 7, 9, 0),
                    Value = 34.4
                },
                new Observation
                {
                    TimeSeries = timeSeries3,
                    Time = new DateTime(1990, 7, 24, 7, 9, 15),
                    Value = 32.5
                },
            };

            context.ObservingFacilities.AddRange(observingFacilities);
            context.TimeSeries.AddRange(timeSeries);
            context.Observations.AddRange(observations);
            context.SaveChanges();
        }
    }
}
