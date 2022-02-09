using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Craft.Utils;
using Craft.Logging;
using DMI.SMS.Application;
using DMI.FD.Domain;
using DMI.FD.Domain.IO;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.Application
{
    public enum MeteorologicalStationListGenerationMode
    {
        Meteorological,
        Climate
    }

    public delegate bool ProgressCallback(
        double progress,
        string currentActivity);

    public class Application
    {
        private IUIDataProvider _uiDataProvider;
        private ILogger _logger;

        public IUIDataProvider UIDataProvider => _uiDataProvider;

        // It must be possible for an external component to set the Logger, e.g. in order to override with a decorator
        public ILogger Logger
        {
            get => _logger;
            set => _logger = value;
        }

        public Application(
            IUIDataProvider uiDataProvider,
            ILogger logger)
        {
            _uiDataProvider = uiDataProvider;
            _logger = logger;

            _uiDataProvider.Initialize(logger);
        }

        public async Task MakeBreakfast(
            DateTime? cutDate,
            ProgressCallback progressCallback = null)
        {
            await Task.Run(() =>
            {
                var result = 0.0;
                var currentActivity = "Baking bread";
                var count = 0;
                var total = 317;

                while (count < total)
                {
                    if (count >= 160)
                    {
                        currentActivity = "Poring Milk";
                    }
                    else if (count >= 80)
                    {
                        currentActivity = "Frying eggs";
                    }

                    for (var j = 0; j < 999999999 / 100; j++)
                    {
                        result += 1.0;
                    }

                    count++;

                    if (progressCallback?.Invoke(100.0 * count / total, currentActivity) is true)
                    {
                        break;
                    }
                }
            });
        }

        public async Task ExtractMeteorologicalStations(
            DateTime? cutDate,
            ProgressCallback progressCallback = null)
        {
            await Task.Run(async () =>
            {
                var dataFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..//..//..//..", "Data"));
                var metParameterListFileName = Path.Combine(dataFolder, "metObs_parameter.json");
                var metParameters = FD.Domain.IO.Helpers.ReadParametersFromJsonFile(metParameterListFileName);
                var allParams = metParameters.Select(p => p.parameterId).OrderBy(paramId => paramId).ToList();

                var referenceMapBasedOnObsDB = new Dictionary<string, Dictionary<string, int>>();

                for (var year = 1953; year <= 2021; year++)
                {
                    var referenceFile = new FileInfo(
                        Path.Combine(dataFolder, "ObservationMatrices//Meteorological", $"Meteorological_observations_in_{year}_from_nanoq.dmi.dk.txt"));

                    var referenceMapForCurrentYear = referenceFile.ReadStationTableFromFile();

                    referenceMapBasedOnObsDB.Aggregate(referenceMapForCurrentYear);
                }

                var paramsDictionary = referenceMapBasedOnObsDB.ConvertToParameterListMap(500);

                await GenerateStationListAndAddParameters(
                    allParams,
                    paramsDictionary,
                    MeteorologicalStationListGenerationMode.Meteorological,
                    progressCallback);
            });
        }

        private async Task GenerateStationListAndAddParameters(
            List<string> allParams,
            Dictionary<string, List<string>> paramsDictionary,
            MeteorologicalStationListGenerationMode mode,
            ProgressCallback progressCallback)
        {
            await Task.Run(() =>
            {
                var timeNow = DateTime.UtcNow.AsEpochInMicroSeconds();

                string modeAsString;
                string outputJsonFileName = null;
                string outputCsvFileName = null;
                string outputOGCJsonFileName = null;

                switch (mode)
                {
                    case MeteorologicalStationListGenerationMode.Meteorological:
                        modeAsString = "meteorological";
                        outputJsonFileName = "metObs_station.json";
                        outputCsvFileName = "metObs_station.csv";
                        outputOGCJsonFileName = "ogcmetObs_station.json";
                        break;
                    case MeteorologicalStationListGenerationMode.Climate:
                        modeAsString = "climate";
                        outputJsonFileName = "climaObs_station_oldFormat.json";
                        outputCsvFileName = "climaObs_station.csv";
                        outputOGCJsonFileName = "climaObs_station.json";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Invalid mode for station list generation");
                }

                var message = $"Generating {modeAsString} station list";
                _logger.WriteLine(LogMessageCategory.Information, message, "general");

                if (progressCallback.Invoke(2, ""))
                {
                    return;
                }

                // 3) Generer stierne til output-filerne
                //var outputFolderName = Path.Combine(SettingsViewModel.OutputDataFolder, "Basisdata");
                var outputFolderName = "Basisdata";
                var outputFolder = new DirectoryInfo(outputFolderName);
                if (!outputFolder.Exists)
                {
                    outputFolder.Create();
                }

                //var smsDBHost = "172.25.7.23";
                //var smsDBName = "sms_prod";
                //var smsDBUser = SettingsViewModel.SMSDatabaseUser;
                //var smsDBPassword = SettingsViewModel.SMSDatabasePassword;

                var stationOwners = new List<StationOwner>();

                var stationOwnerCodeForDMI = 0;
                stationOwners.Add(StationOwner.DMI);

                var status = new List<Status> { Status.Active };
                int? limit = null;

                var stationTypes = new List<StationType>
                {
                    StationType.Synop,
                    StationType.GIWS,
                    StationType.Pluvio,
                    StationType.Manuel_nedbør,
                    StationType.Snestation
                };

                // Fetch all rows
                //var stationDataRaw = await _smsDBDataProvider.RetrieveDataFromStationInformationTable(
                //    smsDBHost, smsDBName, smsDBUser, smsDBPassword, null, null, null, null, limit, false);
                var stationDataRaw = UIDataProvider.GetAllStationInformations();

                // Todo: Sørg for at frafiltrere de rækker, der er blacklistet

                // Optionally roll back to given date of interest
                var dateTimeOfInterest = new DateTime(2021, 1, 24, 0, 0, 0);
                stationDataRaw = stationDataRaw.RollbackToPreviousDate(dateTimeOfInterest);

                // Filter out everything that is not current
                var stationData = stationDataRaw
                    .Where(row => row.GdbToDate.Year == 9999)
                    .ToList();

                // Filter out everything that doesn't match criteria, and convert to Frie Data stations
                var stations = stationData
                    .Where(row => row.StationOwner.HasValue && stationOwners.Contains(row.StationOwner.Value))
                    .Where(row => row.Status.HasValue && status.Contains(row.Status.Value))
                    .Where(row => row.Stationtype.HasValue && stationTypes.Contains(row.Stationtype.Value))
                    .Select(s => s.ConvertToFrieDataStation(false))
                    .ToList();

                // Remove stations that should not be included in the given dataset, if any
                if (mode == MeteorologicalStationListGenerationMode.Climate)
                {
                    stations = stations.Where(s => s.stationId != "06183").ToList();
                }

                File.WriteAllLines("station_names.txt", stations.Select(s => s.name));

                // Add historical meteorological stations
                var koldingSneStation = stationData
                    .Single(row => row.StationIDDMI == 23327);


                stations.Add(koldingSneStation.ConvertToFrieDataStation(false));

                // Tilføj Vestervig snestation - den kom ikke med over fra SnowStation tabellen til StationInformation tabellen
                stations.Add(new Station
                {
                    _id = Guid.NewGuid().ToString(),
                    country = "DNK",
                    instrumentParameter = new List<InstrumentParameter>(),
                    location = new Location
                    {
                        latitude = 56.7637,
                        longitude = 8.3207
                    },
                    name = "Vestervig",
                    owner = "DMI",
                    parameterId = new List<string>(),
                    stationId = "21100",
                    status = "Inactive",
                    timeCreated = timeNow,
                    timeOperationFrom = new DateTime(1971, 1, 1).AsEpochInMicroSeconds(),
                    timeOperationTo = new DateTime(2019, 5, 1).AsEpochInMicroSeconds(),
                    timeValidFrom = new DateTime(2019, 5, 1).AsEpochInMicroSeconds(),
                    type = "Manual snow"
                });

                stations = stations.OrderBy(s => s.stationId).ToList();

                // Traverse the list of stations in order to retrieve details for each of them
                var nStations = stations.Count;

                var stationsWithHistory = new List<Station>();
                var stationCount = 0;

                // Traverse all stations and add details for them
                foreach (var station in stations)
                {
                    List<StationInformation> smsStationHistory = null;
                    List<Station> frieDataStationHistory = null;

                    if (true)
                    {
                        if (station.stationId == "21100") // Vestervig (Manual snow)
                        {
                            //message = $"Generating history for station {station.stationId} ({station.name}) (because it is not in the StationInformation table of the SMS database)";
                            //_logger.WriteLine(message, true, true, false);

                            frieDataStationHistory = new List<Station>();

                            frieDataStationHistory.Add(new Station
                            {
                                _id = Guid.NewGuid().ToString(),
                                country = "DNK",
                                instrumentParameter = new List<InstrumentParameter>(),
                                location = new Location
                                {
                                    latitude = 56.7637,
                                    longitude = 8.3207
                                },
                                name = "Vestervig",
                                owner = "DMI",
                                parameterId = new List<string>(),
                                stationId = "21100",
                                status = "Active",
                                timeCreated = timeNow,
                                timeOperationFrom = new DateTime(1971, 1, 1).AsEpochInMicroSeconds(),
                                timeOperationTo = new DateTime(2019, 5, 1).AsEpochInMicroSeconds(),
                                timeValidFrom = new DateTime(1971, 1, 1).AsEpochInMicroSeconds(),
                                timeValidTo = new DateTime(2019, 5, 1).AsEpochInMicroSeconds(),
                                type = "Manual snow"
                            });

                            frieDataStationHistory.Add(station);
                            stationsWithHistory = stationsWithHistory.Concat(frieDataStationHistory).ToList();
                        }
                        else
                        {
                            // Inspect station history
                            smsStationHistory = stationDataRaw
                                .Where(row => row.StationIDDMI == station.stationId.ConvertToSMSStationId())
                                .Where(row => row.Stationtype == station.type.ConvertToStationType())
                                .OrderBy(row => row.GdbFromDate)
                                .ToList();

                            // Also generate FrieData history and dump to file
                            frieDataStationHistory = smsStationHistory.AsFrieDataStationHistory(
                                true,
                                true,
                                true,
                                true,
                                true,
                                true,
                                true,
                                true,
                                true,
                                true,
                                null,
                                null,
                                false,
                                timeNow);

                            stationsWithHistory = stationsWithHistory.Concat(frieDataStationHistory).ToList();
                        }
                    }

                    stationCount++;

                    if (progressCallback.Invoke(2.0 + 96.0 * stationCount / nStations, ""))
                    {
                        return;
                    }
                }

                stations = stationsWithHistory;

                // Strip pluvio stations of WMO properties
                stations
                    .Where(s => s.type == "Pluvio")
                    .ToList().
                    ForEach(s =>
                    {
                        s.wmoCountryCode = null;
                        s.wmoStationId = null;
                    });

                // Trim the lat long coordinates
                stations = stations.Select(s => s.TrimLatLongCoordinates(4)).ToList();

                // Sæt parametre på stationerne eller mere specifikt på alle stationsrækkerne
                if (true)
                {
                    var officialParametersEncountered = new HashSet<string>();
                    var idsOfStationsWithoutParameters = new List<string>();

                    stations.ForEach(s =>
                    {
                        if (paramsDictionary.TryGetValue(s.stationId, out List<string> parameters))
                        {
                            // Tilføj de parametre, som stationen ifølge obsdb måler, til stationen
                            s.parameterId = parameters;

                            s.parameterId.ForEach(p =>
                            {
                                officialParametersEncountered.Add(p);
                            });
                        }
                        else
                        {
                            idsOfStationsWithoutParameters.Add(s.stationId);
                        }
                    });
                }

                // Fix errors and make corrections
                stations.ForEach(s =>
                {
                    if (s.stationId == "23327")
                    {
                        // Country field is empty for Kolding manual snow station
                        s.country = "DNK";
                    }
                });

                stations = stations.OrderBy(s => s.stationId).ToList();

                stations.WriteStationsToJsonFile(outputJsonFileName);

                var allColumns = true;
                var skipInactiveRecords = false;
                stations.WriteStationsToCsvFile(outputCsvFileName, allColumns, skipInactiveRecords);

                var ogcStations = stations
                    .Select(s => s.ConvertToFrieDataOGCMeteorologicalStation(DateTime.UtcNow)).ToList();

                ogcStations.WriteOGCMeteorologicalStationsToJsonFile(outputOGCJsonFileName);

                progressCallback.Invoke(100, "");
            });
        }
    }
}
