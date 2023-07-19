using System;
using DMI.ObsDB.Domain.Entities;

namespace DMI.ObsDB.SimpleDataMigrator
{
    // Limit på 1000 stationer og data fra 1980 til 2000 giver en database på 477928 KB dvs knap en halv GB
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("DMI.ObsDB.SimpleDataMigrator");

            var stationLimit = 999999;
            var unitOfWorkFactory_File = new Persistence.File.UnitOfWorkFactory();
            var unitOfWorkFactory_Sqlite = new Persistence.EntityFrameworkCore.Sqlite.UnitOfWorkFactory();

            using (var unitOfWork_Sqlite = unitOfWorkFactory_Sqlite.GenerateUnitOfWork())
            {
                var observingFacilitCount = unitOfWork_Sqlite.ObservingFacilities.CountAll();

                if (observingFacilitCount > 0)
                {
                    Console.WriteLine("Repository exists and is non-empty - aborting");
                    Console.WriteLine($"  Observing Facilities: {observingFacilitCount}");

                    var timeSeriesCount = unitOfWork_Sqlite.TimeSeries.CountAll();
                    Console.WriteLine($"  Timeseries: {timeSeriesCount}");

                    var observationCount = unitOfWork_Sqlite.Observations.CountAll();
                    Console.WriteLine($"  Observations: {observationCount}");

                    return;
                }
            };

            IEnumerable<ObservingFacility> observingFacilities_File = null;

            using (var unitOfWork_File = unitOfWorkFactory_File.GenerateUnitOfWork())
            {
                Console.WriteLine($"Reading all observing facilities..");

                observingFacilities_File = unitOfWork_File.ObservingFacilities.GetAll();
                observingFacilities_File = observingFacilities_File.Take(stationLimit);
            }

            foreach (var observingFacility_File in observingFacilities_File)
            {
                Console.WriteLine($"Processing {observingFacility_File.StatId}..");

                using (var unitOfWork_File = unitOfWorkFactory_File.GenerateUnitOfWork())
                {
                    var of = unitOfWork_File.ObservingFacilities.GetIncludingTimeSeries(observingFacility_File.Id);

                    if (of.TimeSeries == null)
                    {
                        continue;
                    }

                    observingFacility_File.TimeSeries = of.TimeSeries;
                }

                using (var unitOfWork_Sqlite = unitOfWorkFactory_Sqlite.GenerateUnitOfWork())
                {
                    unitOfWork_Sqlite.ObservingFacilities.Add(observingFacility_File);
                    unitOfWork_Sqlite.Complete();
                }

                foreach (var timeSeries in observingFacility_File.TimeSeries)
                {
                    using (var unitOfWork_File = unitOfWorkFactory_File.GenerateUnitOfWork())
                    {
                        var ts = unitOfWork_File.TimeSeries.GetIncludingObservations(
                            timeSeries.Id);

                        //var ts = unitOfWork_File.TimeSeries.GetIncludingObservations(
                        //    timeSeries.Id,
                        //    new DateTime(1953, 1, 1),
                        //    new DateTime(2000, 1, 1));

                        if (ts.Observations == null)
                        {
                            continue;
                        }

                        using (var unitOfWork_Sqlite = unitOfWorkFactory_Sqlite.GenerateUnitOfWork())
                        {
                            unitOfWork_Sqlite.Observations.AddRange(ts.Observations);
                            unitOfWork_Sqlite.Complete();
                        }
                    }
                }
            }
        }
    }
}