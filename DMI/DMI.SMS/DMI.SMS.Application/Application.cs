using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Craft.Utils;
using Craft.Logging;
using DMI.FD.Domain;
using DMI.FD.Domain.IO;
using DMI.SMS.Domain.Entities;
using DMI.SMS.IO;

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
        private static Dictionary<int, string> _blackListedStationIds = new Dictionary<int, string>
        {
            //{ 6052, "Thyborøn (There is quite a bit of confusion about this one. Ib says it has been owned by DMI since 1961, the according to the sms database, it has been owned by Kystdirektoratet in a period of time). Julia Sommer has so far decided to omit it" },
            { 6189, "DMI Giws Test" },
            { 34231, "Mittarfik Kangerlussuaq (According to Jens Q it is ours until 1st Jan 2020, but Julia says we shouldn't include it)" },
            { 34234, "Mittarfik Sisimiut (According to Jens Q we are in doubt, so we exclude it)" },
            { 34310, "Station Nord (According to Jens Q we are in doubt, so we exclude it)" },
            //{ 20552, "Års Syd (Julia, PO of Frie Data decided to omit this station mid December 2019)" },
            //{ 28280, "Årslev (Replaced by Højby. Julia, PO of Frie Data decided to omit this station mid December 2019)" },
            //{ 32175, "Østerlars (Julia, PO of Frie Data decided to omit this station mid December 2019)" },
            { 06055, "Torsminde (Det er en Synop ejet af KDI, som vi ikke udstiller (vi udstiller kun deres vandstandsstationer)" },
            { 04405, "Qaanaaq Isvp Vest (Tilføjet efter launch af v2)" },
            { 04406, "Qaanaaq Isvp Øst (Tilføjet efter launch af v2)" },
            { 05126, "Karup Vest (Tilføjet efter launch af v2)" },
            { 05127, "Karup Øst (Tilføjet efter launch af v2)" },
            { 21100, "Vestervig (fordi den er genaktiveret, men vi låser lige på releasen fra efteråret 2020)" },
            { 25152, "Esbjerg Havn 0 - teststation ifølge Ib"},
            { 30344, "Kastrup Havn - teststation ifølge Ib"},
            { 25151, "Esbjerg Havn || - den skal vi ikka have med endnu (13-04-2021)" }
        };

        public static Dictionary<int, List<int>> _blackListedStationRowsIdentifiedByObjectId = new Dictionary<int, List<int>>
        {
            { 6051, new List<int>{ 102091, 102092 } },                                                 // Vestervig
            { 6116, new List<int>{ 102273 } },                                                         // Store Jyndevad
            { 6147, new List<int>{ 102325, 102326, 102327, 102329, 102332, 102337, 102338, 102341 } }, // Vindebæk Kyst
            { 6181, new List<int>{ 102447 } },                                                         // Jægersborg
            { 6183, new List<int>{ 102451, 102452, 102454 } },                                         // Drogden Fyr
            { 4220, new List<int>{ 512 } },                                                            // Aasiaat
            { 4320, new List<int>{ 550 } },                                                            // Danmarkshavn
            { 4339, new List<int>{ 377 } },                                                            // Ittoqqortoormiit
            { 4360, new List<int>{ 346 } },                                                            // Roskilde
            { 30414, new List<int>{ 53371 } },                                                         //
            { 24380, new List<int>{ 53376 } },                                                         // Tarm
            { 31063, new List<int>{ 11770 } },                                                         // Rørvig
            { 32175, new List<int>{ 53361, 53362, 53365 } },                                           // Østerlars
        };

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
        }

        public void Initialize()
        {
            Logger?.WriteLine(LogMessageCategory.Debug, "DMI.SMS.UI.WPF - initializing application");

            _uiDataProvider.Initialize(_logger);
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

        public async Task ExportData(
            ProgressCallback progressCallback = null)
        {
            await Task.Run(() =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Exporting data..");
                progressCallback?.Invoke(0.0, "Exporting data");

                UIDataProvider.ExportData("SMSData.json");

                progressCallback?.Invoke(100, "");
                Logger?.WriteLine(LogMessageCategory.Information, "Completed exporting data");
            });
        }

        public async Task ImportData(
            ProgressCallback progressCallback = null)
        {
            await Task.Run(() =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Importing data..");

                var result = 0.0;
                var currentActivity = "Checkpoint A";
                var count = 0;
                var total = 317;

                Logger?.WriteLine(LogMessageCategory.Information, $"  {currentActivity}");

                while (count < total)
                {
                    if (count >= 160)
                    {
                        currentActivity = "Checkpoint C";

                        if (count == 160)
                        {
                            Logger?.WriteLine(LogMessageCategory.Information, $"  {currentActivity}");
                        }
                    }
                    else if (count >= 80)
                    {
                        currentActivity = "Checkpoint B";

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

                Logger?.WriteLine(LogMessageCategory.Information, "Completed importing data");
            });
        }

        public async Task ExtractMeteorologicalStations(
            DateTime? cutDate,
            ProgressCallback progressCallback = null)
        {
            await Task.Run(async () =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Extracting meteorological stations..");
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

                Logger?.WriteLine(LogMessageCategory.Information, "Completed extracting meteorological stations");
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

                // Remove records with blacklisted station ids
                stationDataRaw = stationDataRaw
                    .Where(row => !(row.StationIDDMI.HasValue && 
                                  _blackListedStationIds.Keys.Contains(row.StationIDDMI.Value)))
                    .ToList();

                stationDataRaw = stationDataRaw
                    .Where(row => !(
                        row.StationIDDMI.HasValue && 
                        _blackListedStationRowsIdentifiedByObjectId.Keys.Contains(row.StationIDDMI.Value) &&
                        _blackListedStationRowsIdentifiedByObjectId[row.StationIDDMI.Value].Contains(row.ObjectId)))
                    .ToList();

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
                    Logger?.WriteLine(LogMessageCategory.Information, $"  processing station {station.stationId} ({station.name})..");

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

                            var dataIOHandler = new DataIOHandler();
                            dataIOHandler.WriteStationHistoryToFile(
                                smsStationHistory,
                                $"{station.stationId}_{station.type}_History_SMS.txt");

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
