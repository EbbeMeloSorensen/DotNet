using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Craft.Logging;
using Craft.Utils;
using Craft.Utils.Linq;
using DMI.FD.Domain;
using DMI.FD.Domain.IO;
using DMI.ObsDB.Domain.Entities;
using DMI.SMS.Domain.Entities;
using DMI.SMS.IO;

namespace DMI.Data.Studio.Application
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

        private SMS.Application.IUIDataProvider _smsUiDataProvider;
        private ObsDB.Persistence.IUnitOfWorkFactory _unitOfWorkFactoryObsDB;
        private ILogger _logger;

        public SMS.Application.IUIDataProvider SMSUIDataProvider => _smsUiDataProvider;

        // It must be possible for an external component to set the Logger, e.g. in order to override with a decorator
        public ILogger Logger
        {
            get => _logger;
            set => _logger = value;
        }

        public Application(
            SMS.Application.IUIDataProvider smsUIDataProvider,
            ObsDB.Persistence.IUnitOfWorkFactory unitOfWorkFactoryObsDB,
            ILogger logger)
        {
            _smsUiDataProvider = smsUIDataProvider;
            _unitOfWorkFactoryObsDB = unitOfWorkFactoryObsDB;
            _logger = logger;
        }

        public void Initialize()
        {
            Logger?.WriteLine(LogMessageCategory.Debug, "DMI.SMS.UI.WPF - initializing application");

            _smsUiDataProvider.Initialize(_logger);
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

        public async Task<int> RollADie(
            ProgressCallback progressCallback = null)
        {
            return await Task.Run(() =>
            {
                var random = new Random();
                return random.Next(1, 6);
            });
        }

        public async Task<List<Tuple<DateTime, DateTime>>> ExtractObservationIntervals(
            string nanoqStationId,
            string parameter,
            double maxTolerableDifferenceBetweenTwoObservationsInHours,
            int firstYear,
            int lastYear,
            bool useCacheIfPossible,
            ProgressCallback progressCallback = null)
        {
            return await Task.Run(() =>
            {
                var result = new List<Tuple<DateTime, DateTime>>();
                var cacheName2 = Path.Combine(@"C:\Data\Stations", $"{nanoqStationId}_intervals.txt");
                var file = new FileInfo(cacheName2);

                if (file.Exists && useCacheIfPossible)
                {
                    // Filen er allerede genereret, så læs den og returner
                    using (var streamReader = new StreamReader(cacheName2))
                    {
                        string line;

                        while ((line = streamReader.ReadLine()) != null)
                        {
                            var year1 = int.Parse(line.Substring(0, 4));
                            var month1 = int.Parse(line.Substring(5, 2));
                            var day1 = int.Parse(line.Substring(8, 2));
                            var hour1 = int.Parse(line.Substring(11, 2));
                            var minute1 = int.Parse(line.Substring(14, 2));
                            var second1 = int.Parse(line.Substring(17, 2));

                            var year2 = int.Parse(line.Substring(22, 4));
                            var month2 = int.Parse(line.Substring(27, 2));
                            var day2 = int.Parse(line.Substring(30, 2));
                            var hour2 = int.Parse(line.Substring(33, 2));
                            var minute2 = int.Parse(line.Substring(36, 2));
                            var second2 = int.Parse(line.Substring(39, 2));

                            result.Add(new Tuple<DateTime, DateTime>(
                                new DateTime(year1, month1, day1, hour1, minute1, second1),
                                new DateTime(year2, month2, day2, hour2, minute2, second2)));
                        }

                        return result;
                    }
                }

                Logger?.WriteLine(LogMessageCategory.Debug, $"  Processing station {nanoqStationId}..");

                // Filen er IKKE genereret, så generer den (så det går hurtigt næste gang) og returner listen

                // I første omgang skal vi have fat i chunks. De er muligvis allerede lavet, og i så fald læser vi dem fra cachen
                var chunkCacheName = Path.Combine(@"C:\Data\Stations", $"{nanoqStationId}_chunks.txt");

                var chunkFile = new FileInfo(chunkCacheName);

                var chunks = new Queue<Chunk>();

                if (chunkFile.Exists)
                {
                    chunks = ReadChunksFile(chunkCacheName);
                }
                else
                {
                    // Generate chunks by reading from database

                    TimeSeries timeSeries = null;

                    using (var unitOfWork = _unitOfWorkFactoryObsDB.GenerateUnitOfWork())
                    {
                        // The observing facility might not be there, since observations are retrieved from
                        // a different repository than the stations

                        try
                        {
                            var observingFacility = unitOfWork.ObservingFacilities
                                .GetIncludingTimeSeries(int.Parse(nanoqStationId));

                            if (observingFacility != null && observingFacility.TimeSeries != null)
                            {
                                timeSeries = observingFacility.TimeSeries
                                    .Where(_ => _.ParamId == parameter)
                                    .SingleOrDefault();
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            // Just swallow the exception for now
                            Logger?.WriteLine(LogMessageCategory.Debug, $"    Station {nanoqStationId} was not found in observation repository");
                        }
                    }

                    if (timeSeries == null)
                    {
                        return result;
                    }

                    // Der er åbenbart en tidsserie - nu skal vi hente observationer ud for den, og det gør vi lige år for år
                    //var timeStamps = new List<DateTime>();
                    var nYears = lastYear - firstYear + 1;
                    var yearCount = 0;

                    for (var year = firstYear; year <= lastYear; year++)
                    {
                        var chunkCacheNameForGivenYear = Path.Combine(@"C:\Data\Stations", $"{nanoqStationId}_chunks_{year}.txt");
                        var chunkFileForGivenYear = new FileInfo(chunkCacheNameForGivenYear);

                        Queue<Chunk> chunksForGivenYear;

                        if (chunkFileForGivenYear.Exists)
                        {
                            chunksForGivenYear = ReadChunksFile(chunkCacheNameForGivenYear);

                            //if (chunksForGivenYear.Any() && chunks.Any())
                            //{
                            //    var chunk1 = chunks.Last();
                            //    var chunk2 = chunksForGivenYear.First();

                            //    // Todo: Determine if they should be spliced into one chunk
                            //}
                        }
                        else
                        {
                            var timeStampsForGivenYear = new List<DateTime>();

                            Logger?.WriteLine(LogMessageCategory.Debug, $"    Retrieving observations for {year}..");

                            var startTime = new DateTime(year, 1, 1);
                            var endTime = new DateTime(year, 12, 31, 23, 59, 59, 999);

                            using (var unitOfWork = _unitOfWorkFactoryObsDB.GenerateUnitOfWork())
                            {
                                var ts = unitOfWork.TimeSeries.GetIncludingObservations(
                                    timeSeries.Id, startTime, endTime);

                                //timeStamps.AddRange(ts.Observations.Select(_ => _.Time));
                                timeStampsForGivenYear.AddRange(ts.Observations.Select(_ => _.Time));

                                // Der kan åbenbart være dubletter, så det tager vi lige hånd om
                                timeStampsForGivenYear = timeStampsForGivenYear.Distinct().ToList();
                                timeStampsForGivenYear.Sort();

                                chunksForGivenYear = AnalyzeTimeSeries(timeStampsForGivenYear, out var commonSpacings);
                                WriteChunksFile(chunkCacheNameForGivenYear, commonSpacings, chunksForGivenYear);
                            }
                        }

                        while (chunksForGivenYear.Any())
                        {
                            var chunk = chunksForGivenYear.Dequeue();
                            chunks.Enqueue(chunk);
                        }

                        yearCount++;

                        if (progressCallback != null)
                        {
                            progressCallback.Invoke(0.0 + 100.0 * yearCount / nYears, "");
                        }
                    }
                }

                WriteChunksFile(chunkCacheName, new List<int>(), chunks);

                var dir = new DirectoryInfo(@"C:\Data\Stations");
                foreach (var file2 in dir.EnumerateFiles($"{nanoqStationId}_chunks_*.txt"))
                {
                    file2.Delete();
                }

                // Nu er vi så klar til at generere samlingen af intervaller med udgangspunkt i listen af chunks

                var intervals = new List<Tuple<DateTime, DateTime>>();

                while (chunks.Any())
                {
                    var firstChunkInNewInterval = chunks.Dequeue();
                    var startTime = firstChunkInNewInterval.StartTime;
                    var endTime = firstChunkInNewInterval.EndTime;

                    while(chunks.TryPeek(out var chunk))
                    {
                        var chunkSpacingInDays = (chunk.StartTime - endTime).TotalDays;

                        if (chunkSpacingInDays < 1)
                        {
                            chunks.Dequeue();
                            endTime = chunk.EndTime;
                        }
                        else
                        {
                            break;
                        }
                    }

                    intervals.Add(new Tuple<DateTime, DateTime>(startTime, endTime));
                }

                if (!Directory.Exists(@"C:\Data\Stations"))
                {
                    Directory.CreateDirectory(@"C:\Data\Stations");
                }

                if (true)
                {
                    using (var streamWriter = new StreamWriter(cacheName2))
                    {
                        foreach (var interval in intervals)
                        {
                            var t1 = interval.Item1;
                            var t2 = interval.Item2;

                            WriteInterval(streamWriter, t1, t2);
                            streamWriter.WriteLine();

                            result.Add(new Tuple<DateTime, DateTime>(
                                new DateTime(t1.Year, t1.Month, t1.Day, t1.Hour, t1.Minute, t1.Second),
                                new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second)));
                        }
                    }
                }
                else
                {
                    foreach (var interval in intervals)
                    {
                        var t1 = interval.Item1;
                        var t2 = interval.Item2;

                        result.Add(new Tuple<DateTime, DateTime>(
                            new DateTime(t1.Year, t1.Month, t1.Day, t1.Hour, t1.Minute, t1.Second),
                            new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, t2.Second)));
                    }
                }

                return result;
            });
        }

        public async Task<List<string>> ExtractStations(
            ProgressCallback progressCallback = null)
        {
            return await Task.Run(() =>
            {
                var result = new List<string>();

                using (var unitOfWork = _unitOfWorkFactoryObsDB.GenerateUnitOfWork())
                {
                    try
                    {
                        var observingFacilities = unitOfWork.ObservingFacilities.GetAll();

                        result.AddRange(observingFacilities.Select(_ => $"{_.StatId}"));
                    }
                    catch (InvalidOperationException)
                    {
                        // Just swallow the exception for now
                        Logger?.WriteLine(LogMessageCategory.Debug, $"    Error trying to retrieve stations");
                    }
                }

                return result;
            });
        }

        public async Task ExtractOceanographicalStations(
            DateTime? rollBackDate,
            ProgressCallback progressCallback = null)
        {
            await Task.Run(async () =>
            {
                if (progressCallback != null && progressCallback.Invoke(1, ""))
                {
                    return;
                }

                Logger?.WriteLine(LogMessageCategory.Information, "Extracting oceanographical stations..");
                var dataFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..//..//..//..", "Data"));
                var oceanParameterListFileName = Path.Combine(dataFolder, "oceanObs_parameter.json");
                var oceanParameters = FD.Domain.IO.Helpers.ReadParametersFromJsonFile(oceanParameterListFileName);
                var allParams = oceanParameters.Select(p => p.parameterId).OrderBy(paramId => paramId).ToList();

                var referenceMapBasedOnSeaDB = new Dictionary<string, Dictionary<string, int>>();

                for (var year = 1889; year <= 2020; year++)
                {
                    var referenceFile = new FileInfo(
                        Path.Combine(dataFolder, "ObservationMatrices//Oceanographic", $"Oceanographic_observations_in_{year}_from_nanoq.dmi.dk.txt"));

                    var referenceMapForCurrentYear = referenceFile.ReadStationTableFromFile();

                    // Du laver et nyt dictionary identisk med det gamle, bortset fra at KDI stationer har
                    // det ID, som vi opererer med i Frie Data
                    var referenceMapForCurrentYearWithKDIStationIds = new Dictionary<string, Dictionary<string, int>>();

                    foreach (var kvp in referenceMapForCurrentYear)
                    {
                        // wtf?! jeg var nødt til at udkommentere dette i Brasilien efter at have ryddet op i SMS...
                        //referenceMapForCurrentYearWithKDIStationIds[kvp.Key.ConvertFromSMSStationIdToKDIStationId()] = kvp.Value;
                    }

                    referenceMapBasedOnSeaDB.Aggregate(referenceMapForCurrentYearWithKDIStationIds);
                }

                var paramsDictionary = referenceMapBasedOnSeaDB.ConvertToParameterListMap(500);

                // Generate output file names
                var outputJsonFileName = "oceanObs_station.json";
                var outputCsvFileName = "oceanObs_station.csv";
                var outputOGCJsonFileName = "ogcoceanObs_station.json";

                // Fetch all rows
                var stationDataRaw = RetrieveAllRows(rollBackDate);

                // Configure filters
                var stationTypes = new List<StationType> { StationType.Vandstandsstation };
                var stationOwners = new List<StationOwner> { StationOwner.DMI, StationOwner.Kystdirektoratet };
                var status = new List<Status> { Status.Active };
                int? limit = null;

                // Filter out everything that is not current
                var stationData = stationDataRaw
                    .Where(row => row.GdbToDate.Year == 9999)
                    .ToList();

                // Experiment: Filter out all stations that were added according to new usage rules (this should not affect the stationfetcher)
                stationData = stationData
                    .Where(row => !(row.DateTo.HasValue && row.Status == Status.Active))
                    .ToList();

                // Det giver åbenbart problemer, når denne er sat til true
                //var convertFromDMIStationIdToKDIStationId = true;
                var convertFromDMIStationIdToKDIStationId = true;

                // Filter out everything that doesn't match criteria, and convert to Frie Data stations
                var stations = stationData
                    .Where(row => row.StationOwner.HasValue && stationOwners.Contains(row.StationOwner.Value))
                    .Where(row => row.Status.HasValue && status.Contains(row.Status.Value))
                    .Where(row => row.Stationtype.HasValue && stationTypes.Contains(row.Stationtype.Value))
                    .Select(s => s.ConvertToFrieDataStation(convertFromDMIStationIdToKDIStationId))
                    .ToList();

                // Add historical Tide Gauge stations
                var historicalTideGaugeStationIds = new List<int>
                {
                    20002, // Skagen Havn, som blev nedlagt januar 2021
                    20048,
                    20098,
                    21008,
                    22332,
                    23292,
                    25148,
                    26458,
                    28232,
                    28233,
                    29392,
                    30018,
                    30337,
                    30338,
                    30339,
                    31572,
                    31618
                };

                foreach (var historicalTideGaugeStationId in historicalTideGaugeStationIds)
                {
                    var smsStation = SMSUIDataProvider.FindStationInformations(
                        s => s.StationIDDMI.HasValue && 
                        s.StationIDDMI.Value == historicalTideGaugeStationId &&
                        s.GdbToDate.Year == 9999);

                    var frieDataStation = smsStation.Single().ConvertToFrieDataStation(convertFromDMIStationIdToKDIStationId);

                    stations.Add(frieDataStation);
                }

                stations = stations.OrderBy(s => s.stationId).ToList();

                var stationsWithHistory = new List<Station>();
                var nStations = stations.Count;
                var stationCount = 0;

                // Traverse the list of stations in order to retrieve details for each of them
                foreach (var station in stations)
                {
                    Logger?.WriteLine(LogMessageCategory.Information, $"  processing station {station.stationId} ({station.name})..");

                    List<StationInformation> smsStationHistory = null;

                    var smsStationId = station.stationId.ConvertToSMSStationId();

                    if (historicalTideGaugeStationIds
                        .Except(new int[] { 20002 })
                        .Contains(smsStationId))
                    {
                        // Vi har at gøre med en freak of nature gammel station, som skal rekonstrueres
                        Logger?.WriteLine(LogMessageCategory.Information, $"(skipping {smsStationId} for now)");

                        // Ikke sikker på at dette er rigtigt
                        smsStationHistory = stationDataRaw
                            .Where(row => row.StationIDDMI == smsStationId)
                            .OrderBy(row => row.GdbFromDate)
                            .ToList();
                    }
                    else
                    {
                        smsStationHistory = stationDataRaw
                            .Where(row => row.StationIDDMI == smsStationId)
                            .Where(row => row.Stationtype == StationType.Vandstandsstation)
                            .OrderBy(row => row.GdbFromDate)
                            .ToList();
                    }

                    var dataIOHandler = new DataIOHandler();
                    dataIOHandler.WriteStationHistoryToFile(
                            smsStationHistory,
                            $"{station.stationId}_{station.type}_History_SMS.txt");

                    // Work in progress

                    stationCount++;

                    if (progressCallback != null &&
                        progressCallback.Invoke(2.0 + 96.0 * stationCount / nStations, ""))
                    {
                        return;
                    }
                }

                // Work in progress
                Logger?.WriteLine(LogMessageCategory.Information, "So far so good.. (work in progress)");
                //Logger?.WriteLine(LogMessageCategory.Information, "Completed extracting oceanographical stations");

                if (progressCallback != null && progressCallback.Invoke(100, ""))
                {
                    return;
                }
            });
        }

        public async Task ExtractMeteorologicalStations(
            DateTime? rollBackDate,
            ProgressCallback progressCallback = null)
        {
            await Task.Run(async () =>
            {
                // Mærkeligt nok så kan man stadig se en fuldt grøn bar, hvis man kører den 
                if (progressCallback != null && progressCallback.Invoke(1, ""))
                {
                    return;
                }

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
                    rollBackDate,
                    MeteorologicalStationListGenerationMode.Meteorological,
                    progressCallback);

                Logger?.WriteLine(LogMessageCategory.Information, "Completed extracting meteorological stations");
            });
        }

        private async Task GenerateStationListAndAddParameters(
            List<string> allParams,
            Dictionary<string, List<string>> paramsDictionary,
            DateTime? rollbackDate,
            MeteorologicalStationListGenerationMode mode,
            ProgressCallback? progressCallback)
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

                if (progressCallback != null && progressCallback.Invoke(1, ""))
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

                // Fetch all rows
                //var stationDataRaw = RetrieveAllRows(new DateTime(2021, 1, 24)); 
                var stationDataRaw = RetrieveAllRows(rollbackDate);

                // Configure filters
                var stationTypes = new List<StationType>
                {
                    StationType.Synop,
                    StationType.GIWS,
                    StationType.Pluvio,
                    StationType.Manuel_nedbør,
                    StationType.Snestation
                };

                var stationOwners = new List<StationOwner> { StationOwner.DMI };
                var status = new List<Status> { Status.Active };
                int? limit = null;

                // Filter out everything that is not current
                var stationData = stationDataRaw
                    .Where(row => row.GdbToDate.Year == 9999)
                    .ToList();

                // Experiment: Filter out all stations that were added according to new usage rules (this should not affect the stationfetcher)
                stationData = stationData
                    .Where(row => !(row.DateTo.HasValue && row.Status == Status.Active))
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

                // Add historical meteorological stations
                var koldingSneStation = stationData
                    .SingleOrDefault(row => row.StationIDDMI == 23327);

                if (koldingSneStation != null)
                {
                    stations.Add(koldingSneStation.ConvertToFrieDataStation(false));
                }

                if (false) // temporary
                {
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
                }

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
                                .Where(row => row.StationIDDMI == station.stationId.ConvertFromKDIStationIdToSMSStationId())
                                .Where(row => row.Stationtype == station.type.ConvertToStationType())
                                .OrderBy(row => row.GdbFromDate) // Denne sortering virker ikke, hvis man har tilføjet data i henhold til nye brugsregler -
                                // og hvad er det lige at de "nye brugsregler" er, Ebbe?? ffs mand!
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

                    if (progressCallback != null &&
                        progressCallback.Invoke(2.0 + 96.0 * stationCount / nStations, ""))
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

                progressCallback?.Invoke(100, "");
            });
        }

        private IList<StationInformation> RetrieveAllRows(
            DateTime? rollBackDate)
        {
            // Fetch all rows
            var stationDataRaw = SMSUIDataProvider.GetAllStationInformations();

            if (rollBackDate.HasValue)
            {
                // Roll back to given date of interest
                stationDataRaw = stationDataRaw.RollbackToPreviousDate(rollBackDate.Value);
            }

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

            return stationDataRaw;
        }

        private static List<Tuple<DateTime, DateTime>> ConvertToIntervals(
            List<DateTime> observationTimes,
            double maxTolerableDifferenceBetweenTwoObservationsInHours)
        {
            var result = new List<Tuple<DateTime, DateTime>>();

            if (observationTimes.Count == 0)
            {
                return result;
            }

            var startOfCurrentInterval = observationTimes.First();

            var nObservations = observationTimes.Count;

            for (var i = 1; i < nObservations; i++)
            {
                var t1 = observationTimes[i - 1];
                var t2 = observationTimes[i];
                var diff = t2 - t1;

                if (diff.TotalHours > maxTolerableDifferenceBetweenTwoObservationsInHours)
                {
                    result.Add(new Tuple<DateTime, DateTime>(
                        startOfCurrentInterval, t1));

                    startOfCurrentInterval = t2;
                }
            }

            result.Add(new Tuple<DateTime, DateTime>(
                startOfCurrentInterval, observationTimes.Last()));

            return result;
        }

        public static Queue<Chunk> AnalyzeTimeSeries(
            List<DateTime> observationTimes,
            out List<int> commonSpacings)
        {
            var chunks = new Queue<Chunk>();

            var spacings = new List<int> { 0 };

            spacings.AddRange(observationTimes
                .AdjacenPairs()
                .Select(_ => (int)(_.Item2 - _.Item1).TotalMinutes));

            // Identify frequencies in use
            var spacingOccurrences = new Dictionary<int, int>();
            
            spacings.ForEach(_ =>
            {
                if (_ <= 0) return;

                if (!spacingOccurrences.ContainsKey(_))
                {
                    spacingOccurrences[_] = 0;
                }

                spacingOccurrences[_]++;
            });

            commonSpacings = new List<int>();

            foreach (var kvp in spacingOccurrences)
            {
                // NB: For korte tidsserier kan en spacing, der reelt er mellem forskellige "segmenter" i tidsserien,
                // godt forveksles med en, der svarer til en gængs frekvens

                var percentage = 1.0 * kvp.Value / (observationTimes.Count + 1);

                if (percentage > 0.001)
                {
                    commonSpacings.Add(kvp.Key);
                }
            }

            commonSpacings.Sort();

            var temp = observationTimes.Zip(spacings, (timestamp, spacing) => new
            {
                TimeStamp = timestamp,
                Spacing = spacing
            });

            while (temp.Any())
            {
                var chunk = new Chunk { StartTime = temp.First().TimeStamp };
                temp = temp.Skip(1);

                if (temp.Any() && 
                    commonSpacings.Contains(temp.First().Spacing) &&
                    !(temp.Skip(1).Any() && temp.First().Spacing > temp.Skip(1).First().Spacing))
                {
                    var spacingForChunk = temp.First().Spacing;

                    var extraTimeStampsInChunk = temp
                        .TakeWhile(_ => _.Spacing == spacingForChunk);

                    var count = extraTimeStampsInChunk.Count();

                    chunk.ObservationCount = count + 1;

                    chunk.EndTime = extraTimeStampsInChunk.Any() 
                        ? extraTimeStampsInChunk.Last().TimeStamp 
                        : chunk.StartTime;

                    temp = temp.Skip(count);
                }
                else
                {
                    chunk.ObservationCount = 1;
                    chunk.EndTime = chunk.StartTime;
                }

                chunks.Enqueue(chunk);
            }

            return chunks;
        }

        private static void WriteInterval(
            TextWriter writer,
            DateTime t1,
            DateTime t2)
        {
            var year1 = $"{t1.Year}";
            var month1 = $"{t1.Month}".PadLeft(2, '0');
            var day1 = $"{t1.Day}".PadLeft(2, '0');
            var hour1 = $"{t1.Hour}".PadLeft(2, '0');
            var minute1 = $"{t1.Minute}".PadLeft(2, '0');
            var second1 = $"{t1.Second}".PadLeft(2, '0');

            writer.Write($"{year1}-");
            writer.Write($"{month1}-");
            writer.Write($"{day1} ");
            writer.Write($"{hour1}:");
            writer.Write($"{minute1}:");
            writer.Write($"{second1}");

            writer.Write(" - ");

            var year2 = $"{t2.Year}";
            var month2 = $"{t2.Month}".PadLeft(2, '0');
            var day2 = $"{t2.Day}".PadLeft(2, '0');
            var hour2 = $"{t2.Hour}".PadLeft(2, '0');
            var minute2 = $"{t2.Minute}".PadLeft(2, '0');
            var second2 = $"{t2.Second}".PadLeft(2, '0');

            writer.Write($"{year2}-");
            writer.Write($"{month2}-");
            writer.Write($"{day2} ");
            writer.Write($"{hour2}:");
            writer.Write($"{minute2}:");
            writer.Write($"{second2}");
        }

        private static void WriteChunksFile(
            string fileName,
            List<int> commonSpacings,
            Queue<Chunk> chunks)
        {
            using (var writer = new StreamWriter(fileName))
            {
                writer.Write("Common spacings: ");

                if (commonSpacings.Any())
                {
                    writer.WriteLine(commonSpacings.Select(_ => $"{_}").Aggregate((c, n) => $"{c}, {n}"));
                }
                else
                {
                    writer.WriteLine("None");
                }

                foreach (var chunk in chunks)
                {
                    var spacing = (chunk.EndTime - chunk.StartTime).TotalMinutes / (chunk.ObservationCount - 1);

                    WriteInterval(writer, chunk.StartTime, chunk.EndTime);
                    writer.Write($" {chunk.ObservationCount,5}");

                    if (chunk.ObservationCount > 1)
                    {
                        writer.Write($" {spacing}");
                    }

                    writer.WriteLine();
                }
            }
        }

        private static Queue<Chunk> ReadChunksFile(
            string fileName)
        {
            var chunks = new Queue<Chunk>();

            using (var streamReader = new StreamReader(fileName))
            {
                string line;
                var skipCount = 1;

                while ((line = streamReader.ReadLine()) != null)
                {
                    if (skipCount > 0)
                    {
                        skipCount--;
                        continue;
                    }

                    var startTimeAsText = line.Substring(0, 19);
                    var endTimeAsText = line.Substring(22, 19);
                    var observationCountAsText = line.Substring(42, 5).Trim();

                    if (!startTimeAsText.TryParsingAsDateTime(out var startTime))
                    {
                        throw new InvalidDataException("Apparently the chunks file is corrupt");
                    }

                    if (!endTimeAsText.TryParsingAsDateTime(out var endTime))
                    {
                        throw new InvalidDataException("Apparently the chunks file is corrupt");
                    }

                    var observationCount = int.Parse(observationCountAsText);

                    chunks.Enqueue(new Chunk
                    {
                        StartTime = startTime,
                        EndTime = endTime,
                        ObservationCount = observationCount
                    });
                }
            }

            return chunks;
        }
    }
}
