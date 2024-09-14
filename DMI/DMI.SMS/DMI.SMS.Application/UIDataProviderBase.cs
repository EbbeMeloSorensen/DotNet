using System;
using System.Linq;
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

        public abstract void DeleteStationInformations(
            IList<StationInformation> stationInformation);

        public abstract void DeleteAllStationInformations();

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
            string nanoqStationId,
            string parameter,
            double maxTolerableDifferenceBetweenTwoObservationsInDays)
        {
            return _dataIOHandler.ReadObservationIntervalsForStation(
                nanoqStationId,
                parameter,
                maxTolerableDifferenceBetweenTwoObservationsInDays);
        }

        public List<Tuple<DateTime, double>> ReadObservationsForStation(
            string nanoqStationId,
            string parameter,
            int firstYear,
            int lastYear)
        {
            return _dataIOHandler.ReadObservationsForStation(
                nanoqStationId, parameter, firstYear, lastYear);
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
