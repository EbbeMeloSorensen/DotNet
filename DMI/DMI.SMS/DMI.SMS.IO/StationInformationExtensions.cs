using System.Collections.Generic;
using System.Globalization;
using Craft.Utils;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.IO
{
    public static class StationInformationExtensions
    {
        public static List<string> GenerateHeaderAsListOfString(
            this StationInformation stationInformation)
        {
            return new List<string>
            {
                "gdb_from_date",
                "gdb_to_date",
                "datefrom",
                "dateto",
                "created_date",
                "last_edited_date",
                "stationname",
                "stationid_dmi",
                "stationtype",
                "accessaddress",
                "country",
                "status",
                "stationowner",
                "comment",
                "stationid_icao",
                "referencetomaintenanceagreement",
                "facilityid",
                "si_utm",
                "si_northing",
                "si_easting",
                "si_geo_lat",
                "si_geo_long",
                "serviceinterval",
                "lastservicedate",
                "nextservicedate",
                "addworkforcedate",
                "lastvisitdate",
                "altstationid",
                "wmostationid",
                "regionid",
                "wigosid",
                "wmocountrycode",
                "hha",
                "hhp",
                "wmorbsn",
                "wmorbcn",
                "wmorbsnradio",
                "wgs_lat",
                "wgs_long",
                "objectid",
                "globalid",
                "gdb_archive_oid",
                "created_user",
                "last_edited_user",
            };
        }

        public static List<string> AsListOfStrings(
            this StationInformation stationInformation)
        {
            var result = new List<string>
            {
                stationInformation.GdbFromDate.AsDateTimeString(true),
                stationInformation.GdbToDate.AsDateTimeString(true),
                stationInformation.DateFrom.HasValue ? stationInformation.DateFrom.Value.AsDateString() : "",
                stationInformation.DateTo.HasValue ? stationInformation.DateTo.Value.AsDateString() : "",
                stationInformation.CreatedDate.HasValue ? stationInformation.CreatedDate.Value.AsDateTimeString(false) : "",
                stationInformation.LastEditedDate.HasValue ? stationInformation.LastEditedDate.Value.AsDateTimeString(false) : "",
                string.IsNullOrEmpty(stationInformation.StationName) ? "" : stationInformation.StationName,
                stationInformation.StationIDDMI.HasValue ? stationInformation.StationIDDMI.Value.ToString() : "",
                stationInformation.Stationtype.HasValue ? stationInformation.Stationtype.Value.ToString() : "",
                string.IsNullOrEmpty(stationInformation.AccessAddress) ? "" : stationInformation.AccessAddress,
                stationInformation.Country.HasValue ? stationInformation.Country.Value.ToString() : "",
                stationInformation.Status.HasValue ? stationInformation.Status.Value.ToString() : "",
                stationInformation.StationOwner.HasValue ? stationInformation.StationOwner.Value.ToString() : "",
                string.IsNullOrEmpty(stationInformation.Comment) ? "" : stationInformation.Comment,
                string.IsNullOrEmpty(stationInformation.Stationid_icao) ? "" : stationInformation.Stationid_icao,
                string.IsNullOrEmpty(stationInformation.Referencetomaintenanceagreement) ? "" : stationInformation.Referencetomaintenanceagreement,
                string.IsNullOrEmpty(stationInformation.Facilityid) ? "" : stationInformation.Facilityid,
                stationInformation.Si_utm.HasValue ? stationInformation.Si_utm.Value.ToString() : "",
                stationInformation.Si_northing.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0}", stationInformation.Si_northing.Value) : "",
                stationInformation.Si_easting.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0}", stationInformation.Si_easting.Value) : "",
                stationInformation.Si_geo_lat.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0}", stationInformation.Si_geo_lat.Value) : "",
                stationInformation.Si_geo_long.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0}", stationInformation.Si_geo_long.Value) : "",
                stationInformation.Serviceinterval.HasValue ? stationInformation.Serviceinterval.Value.ToString() : "",
                stationInformation.Lastservicedate.HasValue ? stationInformation.Lastservicedate.Value.AsDateString() : "",
                stationInformation.Nextservicedate.HasValue ? stationInformation.Nextservicedate.Value.AsDateString() : "",
                stationInformation.Addworkforcedate.HasValue ? stationInformation.Addworkforcedate.Value.AsDateString() : "",
                stationInformation.Lastvisitdate.HasValue ? stationInformation.Lastvisitdate.Value.AsDateString() : "",
                string.IsNullOrEmpty(stationInformation.Altstationid) ? "" : stationInformation.Altstationid,
                string.IsNullOrEmpty(stationInformation.Wmostationid) ? "" : stationInformation.Wmostationid,
                string.IsNullOrEmpty(stationInformation.Regionid) ? "" : stationInformation.Regionid,
                string.IsNullOrEmpty(stationInformation.Wigosid) ? "" : stationInformation.Wigosid,
                string.IsNullOrEmpty(stationInformation.Wmocountrycode) ? "" : stationInformation.Wmocountrycode,
                stationInformation.Hha.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0}", stationInformation.Hha.Value) : "",
                stationInformation.Hhp.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0}", stationInformation.Hhp.Value) : "",
                stationInformation.Wmorbsn.HasValue ? stationInformation.Wmorbsn.Value.ToString() : "",
                stationInformation.Wmorbsn.HasValue ? stationInformation.Wmorbsn.Value.ToString() : "",
                stationInformation.Wmorbcn.HasValue ? stationInformation.Wmorbcn.Value.ToString() : "",
                stationInformation.Wmorbsnradio.HasValue ? stationInformation.Wmorbsnradio.Value.ToString() : "",
                stationInformation.Wgs_lat.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0}", stationInformation.Wgs_lat.Value) : "",
                stationInformation.Wgs_long.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0}", stationInformation.Wgs_long.Value) : "",
                stationInformation.ObjectId.ToString(),
                stationInformation.GlobalId,
                stationInformation.GdbArchiveOid.ToString(),
                string.IsNullOrEmpty(stationInformation.CreatedUser) ? "" : stationInformation.CreatedUser,
                string.IsNullOrEmpty(stationInformation.LastEditedUser) ? "" : stationInformation.LastEditedUser
            };

            return result;
        }
    }
}
