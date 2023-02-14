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

        public void GenerateSQLScriptForTurningElevationAngles(
            string fileName)
        {
            var maxGdbArchiveOid = 30055;
            var predicates = new List<Expression<Func<StationInformation, bool>>>();

            var maxDate = new DateTime(9999, 12, 31, 23, 59, 59);
            predicates.Add(s => s.GdbToDate == maxDate);

            var stationInformationRowsRaw = FindStationInformations(predicates);

            var stationInformationRowsFiltered = stationInformationRowsRaw
                .Where(_ => _.Status == Status.Active)
                //.Where(_ => _.Stationtype == StationType.Bølgestation)
                //.Where(_ => _.Stationtype == StationType.GIWS)
                //.Where(_ => _.Stationtype == StationType.Historisk_stationstype)
                //.Where(_ => _.Stationtype == StationType.Lynpejlestation)
                //.Where(_ => _.Stationtype == StationType.Manuel_nedbør)
                //.Where(_ => _.Stationtype == StationType.Pluvio)
                //.Where(_ => _.Stationtype == StationType.Radiosonde)
                //.Where(_ => _.Stationtype == StationType.Snestation)
                //.Where(_ => _.Stationtype == StationType.Strømstation)
                //.Where(_ => _.Stationtype == StationType.SVK_gprs)
                .Where(_ => _.Stationtype == StationType.Synop)
                //.Where(_ => _.Stationtype == StationType.Vandstandsstation)
                //.Where(_ => _.Stationtype == StationType.Radar)
                //.Where(_ => _.Stationtype == null)
                .Where(_ => _.StationIDDMI != 4418) // Vi skal ikke have Summit med
                .OrderBy(_ => _.StationIDDMI)
                .ToList();

            using (var streamWriter = new StreamWriter(fileName))
            {
                var now = DateTime.UtcNow;
                var nowAsString = now.AsDateTimeString(true);

                streamWriter.WriteLine($"---------------------------------------------------------------------------------------");
                streamWriter.WriteLine($"--                            STATIONS IN TOTAL: {stationInformationRowsFiltered.Count}");
                streamWriter.WriteLine($"---------------------------------------------------------------------------------------");

                var gdbArchiveOid = maxGdbArchiveOid;
                foreach(var _ in stationInformationRowsFiltered)
                {
                    streamWriter.WriteLine($"--Station: {_.StationIDDMI} ({_.StationName})");

                    var updateQuery = $"UPDATE sde.stationinformation SET gdb_to_date = '{nowAsString}' WHERE globalid = '{_.GlobalId}' AND gdb_to_date = '9999-12-31 23:59:59';";
                    //streamWriter.WriteLine(updateQuery);

                    var sb = new StringBuilder(
                        "INSERT INTO sde.stationinformation(" +
                        "objectid, stationname, stationid_dmi, stationtype, accessaddress, country, status, datefrom, dateto, " +
                        "stationowner, comment, stationid_icao, referencetomaintenanceagreement, facilityid, si_utm, si_northing, " +
                        "si_easting, si_geo_lat, si_geo_long, serviceinterval, lastservicedate, nextservicedate, addworkforcedate, " +
                        "globalid, shape, created_user, created_date, last_edited_user, last_edited_date, gdb_archive_oid, " +
                        "gdb_from_date, gdb_to_date, lastvisitdate, altstationid, wmostationid, regionid, wigosid, wmocountrycode, " +
                        "hha, hhp, wmorbsn, wmorbcn, wmorbsnradio, wgs_lat, wgs_long) " +
                        "VALUES (");

                    // "objectid, stationname, stationid_dmi, stationtype, accessaddress, country, status, datefrom, dateto"
                    var stationName = string.IsNullOrEmpty(_.StationName) ? "null" : $"\'{_.StationName}\'";
                    var stationid_dmi = _.StationIDDMI.HasValue ? _.StationIDDMI.ToString() : "null";
                    var stationtype = _.Stationtype.HasValue ? _.Stationtype.Value.ConvertToStationTypeCode().ToString() : "null";
                    var accessaddress = string.IsNullOrEmpty(_.AccessAddress) ? "null" : $"\'{_.AccessAddress}\'";
                    var country = _.Country.HasValue ? _.Country.Value.ConvertToCountryCode().ToString() : "null";
                    var status = _.Status.HasValue ? _.Status.Value.ConvertToStatusCode().ToString() : "null";
                    var datefrom = _.DateFrom.HasValue ? $"\'{_.DateFrom.Value.AsDateTimeString(false)}\'" : "null";
                    var dateto = _.DateTo.HasValue ? $"\'{_.DateTo.Value.AsDateTimeString(false)}\'" : "null";
                    // "stationowner, comment, stationid_icao, referencetomaintenanceagreement, facilityid, si_utm, si_northing"
                    var stationowner = _.StationOwner.HasValue ? _.StationOwner.Value.ConvertToStationOwnerCode().ToString() : "null";
                    var comment = string.IsNullOrEmpty(_.Comment) ? "null" : $"\'{_.Comment}\'";
                    var stationid_icao = string.IsNullOrEmpty(_.Stationid_icao) ? "null" : $"\'{_.Stationid_icao}\'";
                    var referencetomaintenanceagreement = string.IsNullOrEmpty(_.Referencetomaintenanceagreement) ? "null" : $"\'{_.Referencetomaintenanceagreement}\'";
                    var facilityid = string.IsNullOrEmpty(_.Facilityid) ? "null" : $"\'{_.Facilityid}\'";
                    var si_utm = _.Si_utm.HasValue ? _.Si_utm.Value.ToString() : "null";
                    var si_northing = _.Si_northing.HasValue ? _.Si_northing.Value.ToString() : "null";
                    // "si_easting, si_geo_lat, si_geo_long, serviceinterval, lastservicedate, nextservicedate, addworkforcedate"
                    var si_easting = _.Si_easting.HasValue ? _.Si_easting.Value.ToString() : "null";
                    var si_geo_lat = _.Si_geo_lat.HasValue ? _.Si_geo_lat.Value.ToString() : "null";
                    var si_geo_long = _.Si_geo_long.HasValue ? _.Si_geo_long.Value.ToString() : "null";
                    var serviceinterval = _.Serviceinterval.HasValue ? _.Serviceinterval.Value.ToString() : "null";
                    var lastservicedate = _.Lastservicedate.HasValue ? $"\'{_.Lastservicedate.Value.AsDateTimeString(true)}\'" : "null";
                    var nextservicedate = _.Nextservicedate.HasValue ? $"\'{_.Nextservicedate.Value.AsDateTimeString(true)}\'" : "null";
                    var addworkforcedate = _.Addworkforcedate.HasValue ? $"\'{_.Addworkforcedate.Value.AsDateTimeString(true)}\'" : "null";
                    // "globalid, shape, created_user, created_date, last_edited_user, last_edited_date, gdb_archive_oid"
                    var shape = string.IsNullOrEmpty(_.Shape) ? "null" : $"\'{_.Shape}\'";
                    var created_user = string.IsNullOrEmpty(_.CreatedUser) ? "null" : $"\'{_.CreatedUser}\'";
                    var created_date = _.CreatedDate.HasValue ? $"\'{_.CreatedDate.Value.AsDateTimeString(true)}\'" : "null";
                    var last_edited_user = string.IsNullOrEmpty(_.LastEditedUser) ? "null" : $"\'{_.LastEditedUser}\'";
                    var last_edited_date = _.LastEditedDate.HasValue ? $"\'{_.LastEditedDate.Value.AsDateTimeString(true)}\'" : "null";
                    // "gdb_from_date, gdb_to_date, lastvisitdate, altstationid, wmostationid, regionid, wigosid, wmocountrycode"
                    var lastvisitdate = _.Lastvisitdate.HasValue ? $"\'{_.Lastvisitdate.Value.AsDateTimeString(true)}\'" : "null";
                    var altstationid = string.IsNullOrEmpty(_.Altstationid) ? "null" : $"\'{_.Altstationid}\'";
                    var wmostationid = string.IsNullOrEmpty(_.Wmostationid) ? "null" : $"\'{_.Wmostationid}\'";
                    var regionid = string.IsNullOrEmpty(_.Regionid) ? "null" : $"\'{_.Regionid}\'";
                    var wigosid = string.IsNullOrEmpty(_.Wigosid) ? "null" : $"\'{_.Wigosid}\'";
                    var wmocountrycode = string.IsNullOrEmpty(_.Wmocountrycode) ? "null" : $"\'{_.Wmocountrycode}\'";
                    // "hha, hhp, wmorbsn, wmorbcn, wmorbsnradio, wgs_lat, wgs_long)"
                    var hha = _.Hha.HasValue ? _.Hha.Value.ToString() : "null";
                    var hhp = _.Hhp.HasValue ? _.Hhp.Value.ToString() : "null";
                    var wmorbsn = _.Wmorbsn.HasValue ? _.Wmorbsn.Value.ToString() : "null";
                    var wmorbcn = _.Wmorbcn.HasValue ? _.Wmorbcn.Value.ToString() : "null";
                    var wmorbsnradio = _.Wmorbsnradio.HasValue ? _.Wmorbsnradio.Value.ToString() : "null";
                    var wgs_lat = _.Wgs_lat.HasValue ? _.Wgs_lat.Value.ToString() : "null";
                    var wgs_long = _.Wgs_long.HasValue ? _.Wgs_long.Value.ToString() : "null";

                    // new values
                    var temp = stationid_dmi;
                    if (temp.Length == 4)
                    {
                        temp = "0" + temp;
                    }
                    wigosid = $"\'0-208-0-{temp}\'";
                    gdbArchiveOid = gdbArchiveOid + 1;
                    last_edited_user = "\'ebs@dmi.dk\'";

                    sb.Append($"{_.ObjectId}, ");
                    sb.Append($"{stationName}, ");
                    sb.Append($"{stationid_dmi}, ");
                    sb.Append($"{stationtype}, ");
                    sb.Append($"{accessaddress}, ");
                    sb.Append($"{country}, ");
                    sb.Append($"{status}, ");
                    sb.Append($"{datefrom}, ");
                    sb.Append($"{dateto}, ");
                    sb.Append($"{stationowner}, ");
                    sb.Append($"{comment}, ");
                    sb.Append($"{stationid_icao}, ");
                    sb.Append($"{referencetomaintenanceagreement}, ");
                    sb.Append($"{facilityid}, ");
                    sb.Append($"{si_utm}, ");
                    sb.Append($"{si_northing}, ");
                    sb.Append($"{si_easting}, ");
                    sb.Append($"{si_geo_lat}, ");
                    sb.Append($"{si_geo_long}, ");
                    sb.Append($"{serviceinterval}, ");
                    sb.Append($"{lastservicedate}, ");
                    sb.Append($"{nextservicedate}, ");
                    sb.Append($"{addworkforcedate}, ");
                    sb.Append($"\'{_.GlobalId}\', ");
                    sb.Append($"{shape}, ");
                    sb.Append($"{created_user}, ");
                    sb.Append($"{created_date}, ");
                    sb.Append($"{last_edited_user}, ");
                    sb.Append($"{last_edited_date}, ");
                    sb.Append($"{gdbArchiveOid}, ");
                    sb.Append($"\'{nowAsString}\', ");
                    sb.Append($"\'{_.GdbToDate.AsDateTimeString(true)}\', ");
                    sb.Append($"{lastvisitdate}, ");
                    sb.Append($"{altstationid}, ");
                    sb.Append($"{wmostationid}, ");
                    sb.Append($"{regionid}, ");
                    sb.Append($"{wigosid}, ");
                    sb.Append($"{wmocountrycode}, ");
                    sb.Append($"{hha}, ");
                    sb.Append($"{hhp}, ");
                    sb.Append($"{wmorbsn}, ");
                    sb.Append($"{wmorbcn}, ");
                    sb.Append($"{wmorbsnradio}, ");
                    sb.Append($"{wgs_lat}, ");
                    sb.Append($"{wgs_long}");

                    sb.Append(");");

                    //streamWriter.WriteLine(sb.ToString());
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
