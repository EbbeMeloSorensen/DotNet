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
    public static class DateTimeExtensions
    {
        public static string AsDateTimeString(
            this DateTime dateTime,
            bool includeMilliseconds)
        {
            var year = dateTime.Year;
            var month = dateTime.Month.ToString().PadLeft(2, '0');
            var day = dateTime.Day.ToString().PadLeft(2, '0');

            var hour = dateTime.Hour.ToString().PadLeft(2, '0');
            var minute = dateTime.Minute.ToString().PadLeft(2, '0');
            var second = dateTime.Second.ToString().PadLeft(2, '0');

            var result = $"{year}-{month}-{day} {hour}:{minute}:{second}";

            if (includeMilliseconds)
            {
                var millisecond = dateTime.Millisecond.ToString().PadLeft(3, '0');

                result += $".{millisecond}";
            }

            return result;
        }
    }

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

        public void GenerateSQLScriptForTurningElevationAngles(
            string fileName)
        {
            var predicates = new List<Expression<Func<StationInformation, bool>>>();

            var maxDate = new DateTime(9999, 12, 31, 23, 59, 59);
            predicates.Add(s => s.GdbToDate == maxDate);

            var stationInformationRowsRaw = FindStationInformations(predicates);

            var stationInformationRowsFiltered = stationInformationRowsRaw
                .Where(_ => _.Status == Status.Active)
                .Where(_ => _.Stationtype == StationType.Bølgestation)
                .ToList();

            using (var streamWriter = new StreamWriter(fileName))
            {
                var now = DateTime.UtcNow;
                var nowAsString = now.AsDateTimeString(false);

                streamWriter.WriteLine($"---------------------------------------------------------------------------------------");
                streamWriter.WriteLine($"--                            STATIONS IN TOTAL: {stationInformationRowsFiltered.Count}");
                streamWriter.WriteLine($"---------------------------------------------------------------------------------------");

                stationInformationRowsFiltered
                    //.Take(1)
                    //.ToList()
                    .ForEach(_ =>
                    {
                        streamWriter.WriteLine($"--Station: {_.StationIDDMI} ({_.StationName})");

                        var updateQuery = $"UPDATE sde.stationinformation SET gdb_to_date = '{nowAsString}' WHERE globalid = '{_.GlobalId}' AND gdb_to_date = '9999-12-31 23:59:59';";
                        streamWriter.WriteLine(updateQuery);

                        var sb = new StringBuilder(
                            "INSERT INTO sde.stationinformation(" +
                            "objectid, stationname, stationid_dmi, stationtype, accessaddress, country, status, datefrom, dateto, " +
                            "stationowner, comment, stationid_icao, referencetomaintenanceagreement, facilityid, si_utm, si_northing, " +
                            "si_easting, si_geo_lat, si_geo_long, serviceinterval, lastservicedate, nextservicedate, addworkforcedate, " +
                            "globalid, shape, created_user, created_date, last_edited_user, last_edited_date, gdb_archive_oid, " +
                            "gdb_from_date, gdb_to_date, lastvisitdate, altstationid, wmostationid, regionid, wigosid, wmocountrycode, " +
                            "hha, hhp, wmorbsn, wmorbcn, wmorbsnradio, wgs_lat, wgs_long) " +
                            "VALUES (");

                        var stationName = string.IsNullOrEmpty(_.StationName) ? "null" : $"\'{_.StationName}\'";
                        var stationid_dmi = _.StationIDDMI.HasValue ? _.StationIDDMI.ToString() : "null";
                        var stationtype = _.Stationtype.HasValue ? _.Stationtype.Value.ConvertToStationTypeCode().ToString() : "null";
                        var accessaddress = string.IsNullOrEmpty(_.AccessAddress) ? "null" : $"\'{_.AccessAddress}\'";
                        var country = _.Country.HasValue ? _.Country.Value.ConvertToCountryCode().ToString() : "null";
                        var status = _.Status.HasValue ? _.Status.Value.ConvertToStatusCode().ToString() : "null";
                        var datefrom = _.DateFrom.HasValue ? $"\'{_.DateFrom.Value.AsDateString()}\'" : "null";
                        var dateto = _.DateTo.HasValue ? $"\'{_.DateTo.Value.AsDateString()}\'" : "null";

                        sb.Append($"{_.ObjectId}, ");
                        sb.Append($"{stationName}, ");
                        sb.Append($"{stationid_dmi}, ");
                        sb.Append($"{stationtype}, ");
                        sb.Append($"{accessaddress}, ");
                        sb.Append($"{country}, ");
                        sb.Append($"{status}, ");
                        sb.Append($"{datefrom}, ");
                        sb.Append($"{dateto}, ");

                        sb.Append(")");

                        streamWriter.WriteLine(sb.ToString());
                    });
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
