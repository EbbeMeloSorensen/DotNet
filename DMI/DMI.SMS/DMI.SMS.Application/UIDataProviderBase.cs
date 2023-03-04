using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Craft.Logging;
using DMI.SMS.Domain.Entities;
using DMI.SMS.IO;
using DMI.SMS.Domain;

namespace DMI.SMS.Application
{
    public abstract class UIDataProviderBase : IUIDataProvider
    {
        protected ILogger _logger;
        private readonly IDataIOHandler _dataIOHandler;

        protected UIDataProviderBase(
            IDataIOHandler dataIOHandler)
        {
            _dataIOHandler = dataIOHandler;
        }

        public virtual void Initialize(
            ILogger logger)
        {
            _logger = logger;
        }

        public abstract Task<bool> CheckConnection();

        public abstract void CreateStationInformation(
            StationInformation stationInformation,
            bool assignUniqueObjectId);

        public abstract StationInformation GetStationInformation(
            int id);

        public abstract IList<StationInformation> GetAllStationInformations();

        public abstract IList<StationInformation> FindStationInformations(
            Expression<Func<StationInformation, bool>> predicate);

        public abstract IList<StationInformation> FindStationInformations(
            IList<Expression<Func<StationInformation, bool>>> predicates);

        public abstract int CountAllStationInformations();

        public abstract int CountStationInformations(
            Expression<Func<StationInformation, bool>> predicate);

        public abstract int CountStationInformations(
            IList<Expression<Func<StationInformation, bool>>> predicates);

        public abstract void UpdateStationInformation(
            StationInformation stationInformation);

        public abstract void UpdateStationInformations(
            IList<StationInformation> people);

        public abstract void DeleteStationInformation(
            StationInformation stationInformation);

        public abstract void DeleteStationInformations(
            IList<StationInformation> stationInformation);

        public abstract void DeleteAllStationInformations();

        public void ExportData(
            string fileName,
            IList<Expression<Func<StationInformation, bool>>> predicates)
        {
            var extension = Path.GetExtension(fileName)?.ToLower();

            if (extension == null)
            {
                throw new ArgumentException();
            }

            IList<StationInformation> stationInformations;

            if (predicates == null || predicates.Count == 0)
            {
                _logger?.WriteLine(LogMessageCategory.Information, $"  Retrieving all stationinformation records from repository..");
                stationInformations = GetAllStationInformations();
            }
            else
            {
                _logger?.WriteLine(LogMessageCategory.Information, $"  Retrieving matching stationinformation records from repository..");
                stationInformations = FindStationInformations(predicates);
            }

            _logger?.WriteLine(LogMessageCategory.Information, $"  Retrieved {stationInformations.Count} stationinformation records");

            switch (extension)
            {
                case ".xml":
                {
                    _dataIOHandler.ExportDataToXML(stationInformations, fileName);
                    _logger?.WriteLine(LogMessageCategory.Information, 
                        $"  Exported {stationInformations.Count} stationinformation records to xml file");
                    break;
                }
                case ".json":
                {
                    _dataIOHandler.ExportDataToJson(stationInformations, fileName);
                    _logger?.WriteLine(LogMessageCategory.Information,
                        $"  Exported {stationInformations.Count} stationinformation records to json file");
                    break;
                }
                default:
                {
                    throw new ArgumentException();
                }
            }
        }

        public void GenerateSQLScriptForAddingWigosIDs(
            string fileName)
        {
            var predicates = new List<Expression<Func<StationInformation, bool>>>();

            var maxDate = new DateTime(9999, 12, 31, 23, 59, 59);
            predicates.Add(s => s.GdbToDate == maxDate);

            var stationInformationRowsRaw = FindStationInformations(predicates);

            var stationInformationRowsFiltered = stationInformationRowsRaw
                .Where(_ => _.Status == Status.Active)
                .Where(_ => _.Stationtype == StationType.Bølgestation)           //   (5) OK
                //.Where(_ => _.Stationtype == StationType.GIWS)                   //  (17) OK
                //.Where(_ => _.Stationtype == StationType.Historisk_stationstype) //   (2) OK
                //.Where(_ => _.Stationtype == StationType.Lynpejlestation)        //   (6) OK
                //.Where(_ => _.Stationtype == StationType.Manuel_nedbør)          //   (6) OK
                //.Where(_ => _.Stationtype == StationType.Pluvio)                 //  (76) OK
                //.Where(_ => _.Stationtype == StationType.Radiosonde)             //   (8) OK
                //.Where(_ => _.Stationtype == StationType.Snestation)             //  (71) OK
                //.Where(_ => _.Stationtype == StationType.Strømstation)           //   (3) OK
                //.Where(_ => _.Stationtype == StationType.SVK_gprs)               // (191) OK
                //.Where(_ => _.Stationtype == StationType.Synop)                  // (184) OK (NB Summit er ikke med)
                //.Where(_ => _.Stationtype == StationType.Vandstandsstation)      // (159) OK
                //.Where(_ => _.Stationtype == StationType.Radar)                  //   (5) OK   
                //.Where(_ => _.Stationtype == null)
                .Where(_ => _.StationIDDMI != 4418) // Vi skal ikke have Summit med
                .OrderBy(_ => _.StationIDDMI)
                .ToList();

            using (var streamWriter = new StreamWriter(fileName))
            {
                var now = DateTime.UtcNow;

                streamWriter.WriteLine($"---------------------------------------------------------------------------------------");
                streamWriter.WriteLine($"--                            STATIONS IN TOTAL: {stationInformationRowsFiltered.Count}");
                streamWriter.WriteLine($"---------------------------------------------------------------------------------------");

                foreach(var _ in stationInformationRowsFiltered)
                {
                    streamWriter.WriteLine($"--Station: {_.StationIDDMI} ({_.StationName})");

                    var temp = _.StationIDDMI.ToString();

                    if (temp.Length == 4)
                    {
                        temp = "0" + temp;
                    }

                    var wigosid = $"\'0-208-0-{temp}\'";

                    var updateQuery = $"UPDATE sde.stationinformation_evw SET wigosid = {wigosid} WHERE globalid = '{_.GlobalId}';";
                    streamWriter.WriteLine(updateQuery);
                };
            }
        }

        public void ImportData(string fileName)
        {
            //_logger.WriteLineAndStartStopWatch("Importing data..");
            var extension = Path.GetExtension(fileName)?.ToLower();

            if (extension == null)
            {
                throw new ArgumentException();
            }

            IList<StationInformation> stationInformations;

            switch (extension)
            {
                case ".xml":
                {
                    _dataIOHandler.ImportDataFromXML(
                        fileName, out stationInformations);
                    break;
                }
                case ".json":
                {
                    _dataIOHandler.ImportDataFromJson(
                        fileName, out stationInformations);
                    break;
                }
                default:
                {
                    throw new ArgumentException();
                }
            }

            LoadStationInformations(stationInformations);

            //_logger.StopStopWatchAndWriteLine($"Completed importing {stationInformations.Count} stationInformation rows");
        }

        public List<Tuple<DateTime, DateTime>> ReadObservationIntervalsForStation(
            string directoryName,
            string searchPattern,
            double maxTolerableDifferenceBetweenTwoObservationsInDays)
        {
            return _dataIOHandler.ReadObservationIntervalsForStation(
                directoryName,
                searchPattern,
                maxTolerableDifferenceBetweenTwoObservationsInDays);
        }

        public event EventHandler<StationInformationEventArgs> StationInformationCreated;
        public event EventHandler<StationInformationsEventArgs> StationInformationsUpdated;
        public event EventHandler<StationInformationsEventArgs> StationInformationsDeleted;

        protected abstract void LoadStationInformations(
            IList<StationInformation> stationInformations);

        protected virtual void OnStationInformationCreated(
            StationInformation stationInformation)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            var handler = StationInformationCreated;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, new StationInformationEventArgs(stationInformation));
            }
        }

        protected virtual void OnStationInformationsUpdated(
            IEnumerable<StationInformation> stationInformations)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            var handler = StationInformationsUpdated;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, new StationInformationsEventArgs(stationInformations));
            }
        }

        protected virtual void OnPeopleDeleted(
            IEnumerable<StationInformation> stationInformations)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            var handler = StationInformationsDeleted;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, new StationInformationsEventArgs(stationInformations));
            }
        }
    }
}
