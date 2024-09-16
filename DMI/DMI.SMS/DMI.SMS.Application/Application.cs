using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Craft.Logging;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Persistence;
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
        private static readonly DateTime _maxDate = new DateTime(9999, 12, 31, 23, 59, 59);

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

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IDataIOHandler _dataIOHandler;
        private ILogger _logger;

        // It must be possible for an external component to set the Logger, e.g. in order to override with a decorator
        public ILogger Logger
        {
            get => _logger;
            set => _logger = value;
        }

        public Application(
            IUnitOfWorkFactory unitOfWorkFactory,
            IDataIOHandler dataIOHandler,
            ILogger logger)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _dataIOHandler = dataIOHandler;
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

        public async Task ListStationInformations(
            ProgressCallback progressCallback = null)
        {
            IList<StationInformation>? stationInformations = null;

            await Task.Run(() =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Retrieving station informations..");
                progressCallback?.Invoke(0.0, "Retrieving station informations");

                using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                {
                    stationInformations = unitOfWork.StationInformations.GetAll().ToList();
                }

                progressCallback?.Invoke(100, "");
            });

            Console.WriteLine();
            stationInformations?.ToList().ForEach(p => Console.WriteLine($"  {p.StationName}"));
        }

        public async Task CreateStationInformation(
            StationInformation stationInformation,
            ProgressCallback progressCallback = null)
        {
            await Task.Run(() =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Creating Station Information..");
                progressCallback?.Invoke(0.0, "Station Information");

                using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                {
                    stationInformation.ObjectId = unitOfWork.StationInformations.GenerateUniqueObjectId();
                    stationInformation.GlobalId = unitOfWork.StationInformations.GenerateUniqueGlobalId();
                    stationInformation.GdbFromDate = DateTime.UtcNow;
                    stationInformation.GdbToDate = _maxDate;
                    unitOfWork.StationInformations.Add(stationInformation);
                    unitOfWork.Complete();
                }

                progressCallback?.Invoke(100, "");
                Logger?.WriteLine(LogMessageCategory.Information, "Completed creating station Information");
            });
        }

        public async Task ListSensorLocations(
            ProgressCallback progressCallback = null)
        {
            IList<SensorLocation>? sensorLocations = null;

            await Task.Run(() =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Retrieving sensor locations..");
                progressCallback?.Invoke(0.0, "Retrieving sensor locations");

                using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                {
                    sensorLocations = unitOfWork.SensorLocations.GetAll().ToList();
                }

                progressCallback?.Invoke(100, "");
            });

            Console.WriteLine();
            sensorLocations?.ToList().ForEach(p => Console.WriteLine($"  {p.StationidDMI}"));
        }

        public async Task CreateSensorLocation(
            SensorLocation sensorLocation,
            ProgressCallback progressCallback = null)
        {
            await Task.Run(() =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Creating Sensor Location..");
                progressCallback?.Invoke(0.0, "Sensor Location");

                using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                {
                    sensorLocation.ObjectId = unitOfWork.SensorLocations.GenerateUniqueObjectId();
                    sensorLocation.GlobalId = unitOfWork.SensorLocations.GenerateUniqueGlobalId();
                    sensorLocation.GdbFromDate = DateTime.UtcNow;
                    sensorLocation.GdbToDate = _maxDate;
                    unitOfWork.SensorLocations.Add(sensorLocation);
                    unitOfWork.Complete();
                }

                progressCallback?.Invoke(100, "");
                Logger?.WriteLine(LogMessageCategory.Information, "Completed creating station Information");
            });
        }

        public async Task ExportData(
            string fileName,
            bool excludeSupercededRows,
            ProgressCallback progressCallback = null)
        {
            await Task.Run(() =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Exporting data..");
                progressCallback?.Invoke(2.0, "Exporting data");

                IList<StationInformation> stationInformations;

                List<Expression<Func<StationInformation, bool>>> predicates = null;

                if (excludeSupercededRows)
                {
                    predicates = new List<Expression<Func<StationInformation, bool>>>()
                    {
                        _ => _.GdbToDate == new DateTime(9999, 12, 31, 23, 59, 59)
                    };
                }

                using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                {
                    stationInformations = predicates == null 
                        ? unitOfWork.StationInformations.GetAll().ToList() 
                        : unitOfWork.StationInformations.Find(predicates).ToList();
                }

                _dataIOHandler.ExportData(stationInformations, fileName);

                progressCallback?.Invoke(100, "");
                Logger?.WriteLine(LogMessageCategory.Information, $"Completed exporting data to file: {fileName}");
            });
        }

        public async Task ImportData(
            string fileName,
            ProgressCallback progressCallback = null)
        {
            await Task.Run(() =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Importing data..");
                progressCallback?.Invoke(2.0, "Importing data");

                _dataIOHandler.ImportData(fileName, out var stationInformations);

                using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                {
                    // We reset the values of the identity field because we're not allowed to explicitly set it.
                    // Notice that this implies that the rows are assigned new primary keys which may be practically unacceptable.
                    unitOfWork.StationInformations.Load(stationInformations.Select(_ =>
                    {
                        _.GdbArchiveOid = 0;
                        return _;
                    }));
                }

                progressCallback?.Invoke(100, "");
                Logger?.WriteLine(LogMessageCategory.Information, "Completed importing data");
            });
        }

        public async Task ClearRepository(
            ProgressCallback progressCallback = null)
        {
            await Task.Run(() =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Clearing Repository..");
                progressCallback?.Invoke(2.0, "Clearing Repository");

                using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                {
                    unitOfWork.StationInformations.Clear();
                    unitOfWork.Complete();
                }

                progressCallback?.Invoke(100, "");
                Logger?.WriteLine(LogMessageCategory.Information, "Completed clearing repository");
            });
        }
    }
}
