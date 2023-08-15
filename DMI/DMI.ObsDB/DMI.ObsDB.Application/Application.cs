using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Craft.Logging;
using DMI.ObsDB.Domain.Entities;
using DMI.ObsDB.Persistence;

namespace DMI.ObsDB.Application
{
    public delegate bool ProgressCallback(
        double progress,
        string currentActivity);

    public class Application
    {
        private ILogger _logger;
        private IUnitOfWorkFactory _unitOfWorkFactoryTargetRepository;

        // It must be possible for an external component to set the Logger, e.g. in order to override with a decorator
        public ILogger Logger
        {
            get => _logger;
            set => _logger = value;
        }

        public Application(
            ILogger logger,
            IUnitOfWorkFactory unitOfWorkFactoryTargetRepository)
        {
            _logger = logger;
            _unitOfWorkFactoryTargetRepository = unitOfWorkFactoryTargetRepository;
        }

        public async Task MakeBreakfast(
            ProgressCallback progressCallback = null)
        {
            await Task.Run(() =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Making breakfast..");

                var result = 0.0;
                var currentActivity = "Baking bread";
                var count = 0;
                var total = 317;

                Logger?.WriteLine(LogMessageCategory.Information, $"  {currentActivity}");

                while (count < total)
                {
                    if (count >= 160)
                    {
                        currentActivity = "Pouring Milk";

                        if (count == 160)
                        {
                            Logger?.WriteLine(LogMessageCategory.Information, $"  {currentActivity}");
                        }
                    }
                    else if (count >= 80)
                    {
                        currentActivity = "Frying eggs";

                        if (count == 80)
                        {
                            Logger?.WriteLine(LogMessageCategory.Information, $"  {currentActivity}");
                        }
                    }

                    for (var j = 0; j < 499999999 / 100; j++)
                    {
                        result += 1.0;
                    }

                    count++;

                    // Hvis brugeren har trykket på Abort knappen, vil dette kald returnere true,
                    // og så skal vi breake
                    if (progressCallback?.Invoke(100.0 * count / total, currentActivity) is true)
                    {
                        break;
                    }
                }

                Logger?.WriteLine(LogMessageCategory.Information, "Completed breakfast");
            });
        }

        public async Task Migrate(
            int? stationLimit,
            int? firstYear,
            int? lastYear,
            ProgressCallback progressCallback = null)
        {
            await Task.Run(() =>
            {
                var currentActivity = "Migrating..";
                progressCallback?.Invoke(0, currentActivity);
                Logger?.WriteLine(LogMessageCategory.Information, currentActivity);

                //var unitOfWorkFactorySourceRepository = new Persistence.File.UnitOfWorkFactory();
                var unitOfWorkFactorySourceRepository = new Persistence.PostgreSQL.UnitOfWorkFactory();

                using (var unitOfWorkTargetRepository = _unitOfWorkFactoryTargetRepository.GenerateUnitOfWork())
                {
                    var observingFacilitCount = unitOfWorkTargetRepository.ObservingFacilities.CountAll();

                    if (observingFacilitCount > 0)
                    {
                        Logger?.WriteLine(LogMessageCategory.Information, 
                            "Repository exists and is non-empty - aborting");

                        Logger?.WriteLine(LogMessageCategory.Information,
                            $"  Observing Facilities: {observingFacilitCount}");

                        Logger?.WriteLine(LogMessageCategory.Information,
                            $"  Timeseries: {unitOfWorkTargetRepository.TimeSeries.CountAll()}");

                        Logger?.WriteLine(LogMessageCategory.Information,
                            $"  Observations: {unitOfWorkTargetRepository.Observations.CountAll()}");

                        return;
                    }
                };

                IEnumerable<ObservingFacility> observingFacilities = null;

                using (var unitOfWorkSourceRepository = unitOfWorkFactorySourceRepository.GenerateUnitOfWork())
                {
                    Logger?.WriteLine(LogMessageCategory.Information,
                        "Reading all observing facilities..");

                    observingFacilities = unitOfWorkSourceRepository.ObservingFacilities.GetAll()
                        // .Where(_ =>
                        //  _.StatId == 601100 || 
                        //  _.StatId == 603000 || 
                        //  _.StatId == 604100 || 
                        //  _.StatId == 607100 || 
                        //  _.StatId == 608100)
                        .OrderBy(_ => _.StatId);

                    // if (stationLimit.HasValue)
                    // {
                    //     observingFacilities = observingFacilities.Take(stationLimit.Value);
                    // }
                }

                var count = 0;
                var total = observingFacilities.Count();
                var timeSeriesRetrieved = 0;
                var done = false;

                foreach (var observingFacility in observingFacilities)
                {
                    currentActivity = $"Processing {observingFacility.StatId}..";
                    Logger?.WriteLine(LogMessageCategory.Information, currentActivity);

                    if (progressCallback?.Invoke(100.0 * count++ / total, currentActivity) is true)
                    {
                        Logger?.WriteLine(LogMessageCategory.Information, "Aborted by user");
                        break;
                    }

                    using (var unitOfWorkSourceRepository = unitOfWorkFactorySourceRepository.GenerateUnitOfWork())
                    {
                        var of = unitOfWorkSourceRepository.ObservingFacilities.GetIncludingTimeSeries(
                            observingFacility.Id);

                        if (of.TimeSeries == null || of.TimeSeries.Count == 0)
                        {
                            Logger?.WriteLine(LogMessageCategory.Information, $"..station {observingFacility.StatId} does not have a temp_dry timeseries");
                            continue;
                        }

                        observingFacility.TimeSeries = of.TimeSeries;
                    }

                    using (var unitOfWorkTargetRepository = _unitOfWorkFactoryTargetRepository.GenerateUnitOfWork())
                    {
                        unitOfWorkTargetRepository.ObservingFacilities.Add(observingFacility);
                        unitOfWorkTargetRepository.Complete();
                    }

                    foreach (var timeSeries in observingFacility.TimeSeries)
                    {
                        using (var unitOfWorkSourceRepository = unitOfWorkFactorySourceRepository.GenerateUnitOfWork())
                        {
                            var ts = firstYear.HasValue && lastYear.HasValue
                                ? unitOfWorkSourceRepository.TimeSeries.GetIncludingObservations(
                                    timeSeries.Id,
                                    new DateTime(firstYear.Value, 1, 1),
                                    new DateTime(lastYear.Value + 1, 1, 1))
                                : unitOfWorkSourceRepository.TimeSeries.GetIncludingObservations(
                                    timeSeries.Id);

                            if (ts.Observations == null || ts.Observations.Count == 0)
                            {
                                Logger?.WriteLine(LogMessageCategory.Information, $"..temp_dry timeseries of station {observingFacility.StatId} apparently has no observations");
                                continue;
                            }

                            using (var unitOfWorkTargetRepository = 
                                _unitOfWorkFactoryTargetRepository.GenerateUnitOfWork())
                            {
                                unitOfWorkTargetRepository.Observations.AddRange(ts.Observations);
                                unitOfWorkTargetRepository.Complete();
                            }

                            Logger?.WriteLine(LogMessageCategory.Information, $"..temp_dry timeseries of station {observingFacility.StatId} includes {ts.Observations.Count} observations");
                        }

                        timeSeriesRetrieved++;

                        if (stationLimit.HasValue && timeSeriesRetrieved >= stationLimit.Value)
                        {
                            done = true;
                            break;
                        }
                    }

                    if (done)
                    {
                        break;
                    }
                }

                currentActivity = "Completed migration";
                progressCallback?.Invoke(100.0, currentActivity);
                Logger?.WriteLine(LogMessageCategory.Information, currentActivity);
            });
        }
    }
}
