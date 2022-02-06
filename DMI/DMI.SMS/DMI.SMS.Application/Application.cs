using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Craft.Utils;
using Craft.Logging;

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
                var allParams = new List<string>();

                //allParams = allParams.Concat(
                //    SettingsViewModel.MeteorologicalParameterListViewModel.GetAllStrings()).ToList();

                allParams = allParams.OrderBy(p => p).ToList();

                var referenceMapBasedOnObsDB = new Dictionary<string, Dictionary<string, int>>();

                // Udkommenteret, fordi vi lige skal have styr på matricerne
                //for (var year = 1953; year <= 2021; year++)
                //{
                //    var referenceFile = new FileInfo(
                //        Path.Combine(SettingsViewModel.InputDataFolder, @"ObservationMatrices\Meteorological", $"Meteorological_observations_in_{year}_from_nanoq.dmi.dk.txt"));

                //    var referenceMapForCurrentYear = referenceFile.ReadStationTableFromFile();

                //    referenceMapBasedOnObsDB.Aggregate(referenceMapForCurrentYear);
                //}

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

                var outputJsonFullFileName = Path.Combine(outputFolderName, outputJsonFileName);
                var outputCsvFullFileName = Path.Combine(outputFolderName, outputCsvFileName);
                var outputOGCJsonFullFileName = Path.Combine(outputFolderName, outputOGCJsonFileName);

                /*
                var stations = new List<Station>();

                var smsDBHost = "172.25.7.23";
                var smsDBName = "sms_prod";
                var smsDBUser = SettingsViewModel.SMSDatabaseUser;
                var smsDBPassword = SettingsViewModel.SMSDatabasePassword;

                List<int> stationOwners = new List<int>();

                var stationOwnerCodeForDMI = 0;
                stationOwners.Add(stationOwnerCodeForDMI);

                List<int> status = new List<int> { 1 };
                int? limit = null;

                var stationTypes = new List<int>();

                var stationTypeCodeForSynop = 0;
                var stationTypeCodeForGiws = 4;
                var stationTypeCodeForPluvio = 5;
                var stationTypeCodeForManualPrecipitation = 12;
                var stationTypeCodeForManualSnow = 14;

                stationTypes.Add(stationTypeCodeForSynop);
                stationTypes.Add(stationTypeCodeForGiws);
                stationTypes.Add(stationTypeCodeForPluvio);
                stationTypes.Add(stationTypeCodeForManualPrecipitation);
                stationTypes.Add(stationTypeCodeForManualSnow);

                // Fetch all rows
                var stationDataRaw = await _smsDBDataProvider.RetrieveDataFromStationInformationTable(
                    smsDBHost, smsDBName, smsDBUser, smsDBPassword, null, null, null, null, limit, false);

                // Optionally roll back to given date of interest
                var dateTimeOfInterest = new DateTime(2021, 1, 24, 0, 0, 0);
                stationDataRaw = stationDataRaw.RollbackToPreviousDate(dateTimeOfInterest);

                // Filter out everything that is not current
                var stationData = stationDataRaw
                    .Where(row => row.gdb_to_date.HasValue && row.gdb_to_date.Value.Year == 9999)
                    .ToList();

                // Filter out everything that doesn't match criteria, and convert to Frie Data stations
                stations = stationData
                    .Where(row => row.stationowner.HasValue && stationOwners.Contains(row.stationowner.Value))
                    .Where(row => row.status.HasValue && status.Contains(row.status.Value))
                    .Where(row => row.stationtype.HasValue && stationTypes.Contains(row.stationtype.Value))
                    .Select(s => s.ConvertToFrieDataStation(false))
                    .ToList();

                // Remove blacklisted stations
                // (Gøres direkte i DAL layeret)
                //stations = stations.Where(s => !_blackListedStations.Keys.Contains(s.stationId)).ToList();

                // Remove stations that should not be included in the given dataset, if any
                if (mode == MeteorologicalStationListGenerationMode.Climate)
                {
                    stations = stations.Where(s => s.stationId != "06183").ToList();
                }

                // Add historical meteorological stations
                // (tilføj nedlagte stationer, der før har været udstillet)
                // NB: Vi skal også have Vestervig med, men den er slet ikke blevet overført fra SnowStation-tabellen
                // og til StationInformation-tabellen

                // Kolding snestation (23327)
                var koldingSnowStation = await _smsDBDataProvider.RetrieveDataFromStationInformationTable(
                    smsDBHost, smsDBName, smsDBUser, smsDBPassword, null, null, new List<int> { 14 }, new List<int> { 23327 }, null, true);

                stations.Add(koldingSnowStation.Single().ConvertToFrieDataStation(false));

                // Roerslev snestation (28032)
                //var roerslevSnowStation = await _smsDBDataProvider.RetrieveDataFromStationInformationTable(
                //    smsDBHost, smsDBName, smsDBUser, smsDBPassword, null, null, new List<int> { 14 }, new List<int> { 28032 }, null, true);

                //stations.Add(roerslevSnowStation.Single().ConvertToFrieDataStation(false));

                // Årslev snestation (28280)
                //var aarslevSnowStation = await _smsDBDataProvider.RetrieveDataFromStationInformationTable(
                //    smsDBHost, smsDBName, smsDBUser, smsDBPassword, null, null, new List<int> { 14 }, new List<int> { 28280 }, null, true);

                //stations.Add(aarslevSnowStation.Single().ConvertToFrieDataStation(false));

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

                StationMasterDetailViewModel.StationMasterViewModels.Clear();
                var stationsWithHistory = new List<Station>();
                var stationCount = 0;

                // Traverse all stations and add details for them
                foreach (var station in stations)
                {
                    List<Domain.SMS.StationInformationRow> smsStationHistory = null;
                    List<Station> frieDataStationHistory = null;
                    List<Domain.SMS.ElevationAnglesRow> elevationAngles = null;

                    if (IncludeStationHistory)
                    {
                        if (station.stationId == "21100") // Vestervig (Manual snow)
                        {
                            message = $"Generating history for station {station.stationId} ({station.name}) (because it is not in the StationInformation table of the SMS database)";
                            _logger.WriteLine(message, true, true, false);

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

                            outputFolderName = Path.Combine(SettingsViewModel.OutputDataFolder, "StationHistory_FD");
                            outputFolder = new DirectoryInfo(outputFolderName);

                            if (!outputFolder.Exists)
                            {
                                outputFolder.Create();
                            }

                            var fileNameFDHistory = $"{station.stationId}_{station.type}_History_FD.txt";
                            frieDataStationHistory.WriteStationHistoryToFile(Path.Combine(outputFolderName, fileNameFDHistory));

                            stationsWithHistory = stationsWithHistory.Concat(frieDataStationHistory).ToList();
                        }
                        else
                        {
                            message = $"Inspecting history for station {station.stationId} ({station.name})..";
                            _logger.WriteLine(message, true, true, false);

                            smsStationHistory = stationDataRaw
                                .Where(row => row.stationid_dmi == station.stationId.ConvertToSMSStationId())
                                .Where(row => row.stationtype == station.type.ConvertToStationTypeCode())
                                .ToList();

                            // Write the full sms history to file
                            outputFolderName = Path.Combine(SettingsViewModel.OutputDataFolder, "StationHistory_SMS");
                            outputFolder = new DirectoryInfo(outputFolderName);

                            if (!outputFolder.Exists)
                            {
                                outputFolder.Create();
                            }

                            var outputDirectory = new DirectoryInfo(Path.Combine(SettingsViewModel.OutputDataFolder, "StationHistory"));

                            if (!outputDirectory.Exists)
                            {
                                outputDirectory.Create();
                            }

                            var fileNameSMSHistory = $"{station.stationId}_{station.type}_History_SMS.txt";
                            smsStationHistory.WriteStationHistoryToFile(Path.Combine(outputDirectory.FullName, fileNameSMSHistory));

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
                                timeNow,
                                _logger);

                            outputFolderName = Path.Combine(SettingsViewModel.OutputDataFolder, "StationHistory_FD");
                            outputFolder = new DirectoryInfo(outputFolderName);

                            if (!outputFolder.Exists)
                            {
                                outputFolder.Create();
                            }

                            var fileNameFDHistory = $"{station.stationId}_{station.type}_History_FD.txt";
                            frieDataStationHistory.WriteStationHistoryToFile(Path.Combine(outputFolderName, fileNameFDHistory));

                            stationsWithHistory = stationsWithHistory.Concat(frieDataStationHistory).ToList();
                        }
                    }

                    var toleranceInSeconds = 10;

                    // Pass a "station with details" object to the StationMasterDetail ViewModel
                    StationMasterDetailViewModel.StationMasterViewModels.Add(new StationMasterViewModel(
                        station.stationId,
                        station.name,
                        station.type,
                        smsStationHistory,
                        frieDataStationHistory,
                        elevationAngles,
                        toleranceInSeconds,
                        null,
                        null));

                    stationCount++;
                    CurrentProgress = (int)System.Math.Round(2.0 + 96.0 * stationCount / nStations);
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

                // Er der forskel på, hvilke stationer du har fra sms og så dem du har i reference-matricerne?
                stations
                    .Select(s => s.stationId).Distinct().ToList()
                    .CompareStringLists(
                        "station ids from sms",
                        paramsDictionary.Keys.ToList(),
                        "station ids from pregenerated reference lists",
                        _logger);

                // Sæt parametre på stationerne eller mere specifikt på alle stationsrækkerne
                // Desuden kontrol af, at:
                // * alle stationer udbyder mindst én officiel parameter
                // * alle officielle parametre er udbudt af mindst én station
                if (IncludeParameters)
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

                            var warningMessage = $"WARNING - NO PARAMETERS FOUND FOR STATION: {s.stationId} ({s.name})..";
                            _logger.WriteLine(warningMessage, true, true, false);
                        }
                    });

                    // Summary, part 1
                    if (idsOfStationsWithoutParameters.Count == 0)
                    {
                        message = $"All {stationCount} stations each deal in at least one of the {allParams.Count} official parameters";
                        _logger.WriteLine(message, true, true, false);
                    }
                    else
                    {
                        var warningMessage = $"WARNING - NO PARAMETERS FOUND FOR {idsOfStationsWithoutParameters.Count} OUT OF {stationCount} STATIONS";
                        _logger.WriteLine(warningMessage, true, true, false);
                    }

                    // Summary, part 2
                    if (officialParametersEncountered.Count == allParams.Count)
                    {
                        message = $"All {allParams.Count} official parameters are each published by at least one station";
                        _logger.WriteLine(message, true, true, false);
                    }
                    else
                    {
                        var warningMessage = $"WARNING - {allParams.Count - officialParametersEncountered.Count} official parameters were not published by any station";
                        _logger.WriteLine(warningMessage, true, true, false);
                    }
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

                if (TrimHistoricalData)
                {
                    stations = stations.AsFrieDataStationHistoryCollapsedToActual();
                }

                stations.WriteStationsToJsonFile(outputJsonFullFileName);
                _logger.WriteLine($"  Saved {outputJsonFullFileName}", true, true, false);

                var allColumns = true;
                var skipInactiveRecords = false;
                stations.WriteStationsToCsvFile(outputCsvFullFileName, allColumns, skipInactiveRecords);
                _logger.WriteLine($"  Saved {outputCsvFullFileName}", true, true, false);

                var ogcStations = stations.Select(s => s.ConvertToFrieDataOGCMeteorologicalStation(DateTime.UtcNow)).ToList();
                ogcStations.WriteOGCMeteorologicalStationsToJsonFile(outputOGCJsonFullFileName);
                _logger.WriteLine($"  Saved {outputOGCJsonFullFileName}", true, true, false);

                message = $"Completed generating {modeAsString} station list ({stationCount} stations)";
                _logger.WriteLine(message, true, true, false);
                CurrentProgress = 100;
                await Task.Delay(100);
                */
            });
        }
    }
}
