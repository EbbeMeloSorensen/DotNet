using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Craft.Logging;
using DMI.SMS.Domain.Entities;
using DMI.SMS.IO;

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

        public abstract void DeleteDeleteStationInformations(
            IList<StationInformation> stationInformation);

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
