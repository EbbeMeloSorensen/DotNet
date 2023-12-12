using Craft.Logging;
using C2IEDM.Persistence;
using C2IEDM.Domain.Entities.Geometry.Locations;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using System;
using System.Linq.Expressions;
using C2IEDM.Domain.Entities.WIGOS.GeospatialLocations;

namespace C2IEDM.Application;

public delegate bool ProgressCallback(
    double progress,
    string currentActivity);

public class Application
{
    private IUnitOfWorkFactory _unitOfWorkFactory;
    private ILogger _logger;

    public ILogger Logger
    {
        get => _logger;
        set => _logger = value;
    }

    public Application(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
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
                    currentActivity = "Poring Milk";

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

                if (progressCallback?.Invoke(100.0 * count / total, currentActivity) is true)
                {
                    break;
                }
            }

            Logger?.WriteLine(LogMessageCategory.Information, "Completed breakfast");
        });
    }

    public async Task ListLocations(
        ProgressCallback progressCallback = null)
    {
        IList<Location>? locations = null;

        await Task.Run(() =>
        {
            Logger?.WriteLine(LogMessageCategory.Information, "Retrieving locations..");
            progressCallback?.Invoke(0.0, "Retrieving locations");

            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                // Todo: Denne hiver alle rækker med, inkl de superseedede - den skal kun tage de gældende rækker med
                locations = unitOfWork.Locations.GetAll().ToList();
            }

            progressCallback?.Invoke(100, "");
        });

        Console.WriteLine();
        locations?.ToList().ForEach(_ => Console.WriteLine($"  {_}"));
    }

    public async Task ListObservingFacilities(
        ProgressCallback progressCallback = null)
    {
        List<KeyValuePair<ObservingFacility, List<GeospatialLocation>>> observingFacilityDictionary = null;

        await Task.Run(() =>
        {
            Logger?.WriteLine(LogMessageCategory.Information, "Retrieving observing facilities..");
            progressCallback?.Invoke(0.0, "Retrieving observing facilities");

            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                // Todo: Kommuniker nogle passende predicates ned
                var observingFacilityPredicates = new List<Expression<Func<ObservingFacility, bool>>>();
                var geospatialLocationPredicates = new List<Expression<Func<GeospatialLocation, bool>>>();

                observingFacilityDictionary = 
                    unitOfWork.ObservingFacilities.FindIncludingGeospatialLocations(
                        observingFacilityPredicates,
                        geospatialLocationPredicates)
                        .ToList();
            }

            progressCallback?.Invoke(100, "");
        });

        Console.WriteLine();

        foreach (var entry in observingFacilityDictionary)
        {
            Console.WriteLine($"  Observing Facility: {entry.Key.Name}");

            foreach (var geospatialLocation in entry.Value)
            {
                Console.WriteLine($"    Location: {geospatialLocation}");
            }
        }
    }
}