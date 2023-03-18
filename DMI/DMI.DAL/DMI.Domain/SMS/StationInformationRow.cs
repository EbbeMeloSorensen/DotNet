using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using DMI.Utils;

namespace DMI.Domain.SMS
{
    public class StationInformationRow
    {
        public int? objectid { get; set; } // ok
        public string stationname { get; set; } // ok
        public int? stationid_dmi { get; set; } // ok
        public int? stationtype { get; set; }      // missing in printout
        public string accessaddress { get; set; } // ok
        public int? country { get; set; } // ok
        public int? status { get; set; } // ok
        public DateTime? datefrom { get; set; }
        public DateTime? dateto { get; set; }
        public int? stationowner { get; set; } // ok
        public string comment { get; set; } // ok
        public string stationid_icao { get; set; }  // ok 
        public string referencetomaintenanceagreement { get; set; } // ok
        public string facilityid { get; set; } // ok
        public int? si_utm { get; set; } // ok
        public double? si_northing { get; set; } // ok
        public double? si_easting { get; set; } // ok
        public double? si_geo_lat { get; set; } // ok
        public double? si_geo_long { get; set; } // ok
        public int? serviceinterval { get; set; } // ok
        public DateTime? lastservicedate { get; set; } // ok
        public DateTime? nextservicedate { get; set; } // ok
        public DateTime? addworkforcedate { get; set; } // ok
        public string globalid { get; set; } // ok
        public string created_user { get; set; } // ok
        public DateTime? created_date { get; set; } // ok
        public string last_edited_user { get; set; } // ok
        public DateTime? last_edited_date { get; set; } // ok
        public int? gdb_archive_oid { get; set; } // ok
        public DateTime? gdb_from_date { get; set; } // ok
        public DateTime? gdb_to_date { get; set; } // ok
        public DateTime? lastvisitdate { get; set; } // ok
        public string altstationid { get; set; } // ok
        public string wmostationid { get; set; } // ok
        public string regionid { get; set; } // ok
        public string wigosid { get; set; } // ok
        public string wmocountrycode { get; set; } // ok
        public double? hha { get; set; } // ok
        public double? hhp { get; set; } // ok
        public int? wmorbsn { get; set; } // ok
        public int? wmorbcn { get; set; } // ok
        public int? wmorbsnradio { get; set; } // ok
        public double? wgs_lat { get; set; } // ok
        public double? wgs_long { get; set; } // ok

        public static List<string> GenerateHeaderAsListOfString()
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

        public List<String> AsListOfStrings()
        {
            var result = new List<string>
            {
                gdb_from_date.HasValue ? gdb_from_date.Value.AsDateTimeString(true) : "",
                gdb_to_date.HasValue ? gdb_to_date.Value.AsDateTimeString(true) : "",
                datefrom.HasValue ? datefrom.Value.AsDateString() : "",
                dateto.HasValue ? dateto.Value.AsDateString() : "",
                created_date.HasValue ? created_date.Value.AsDateTimeString(false) : "",
                last_edited_date.HasValue ? last_edited_date.Value.AsDateTimeString(false) : "",
                stationname,
                stationid_dmi.HasValue ? stationid_dmi.Value.ToString() : "",
                stationtype.HasValue ? stationtype.Value.ToString() : "",
                accessaddress != null ? accessaddress : "",
                country.HasValue ? country.Value.ToString() : "",
                status.HasValue ? status.Value.ToString() : "",
                stationowner.HasValue ? stationowner.Value.ToString() : "",
                comment != null ? comment : "",
                stationid_icao != null ? stationid_icao : "",
                referencetomaintenanceagreement != null ? referencetomaintenanceagreement : "",
                facilityid != null ? facilityid : "",
                si_utm.HasValue ? si_utm.Value.ToString() : "",
                si_northing.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0}", si_northing.Value) : "",
                si_easting.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0}", si_easting.Value) : "",
                si_geo_lat.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0}", si_geo_lat.Value) : "",
                si_geo_long.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0}", si_geo_long.Value) : "",
                serviceinterval.HasValue ? serviceinterval.Value.ToString() : "",
                lastservicedate.HasValue ? lastservicedate.Value.AsDateString() : "",
                nextservicedate.HasValue ? nextservicedate.Value.AsDateString() : "",
                addworkforcedate.HasValue ? addworkforcedate.Value.AsDateString() : "",
                lastvisitdate.HasValue ? lastvisitdate.Value.AsDateTimeString(false) : "",
                altstationid != null ? altstationid : "",
                wmostationid != null ? wmostationid : "",
                regionid != null ? regionid : "",
                wigosid != null ? wigosid : "",
                wmocountrycode != null ? wmocountrycode : "",
                hha.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0}", hha) : "",
                hhp.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0}", hhp) : "",
                wmorbsn.HasValue ? wmorbsn.Value.ToString() : "",
                wmorbcn.HasValue ? wmorbcn.Value.ToString() : "",
                wmorbsnradio.HasValue ? wmorbsnradio.Value.ToString() : "",
                wgs_lat.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0}", wgs_lat) : "",
                wgs_long.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0}", wgs_long) : "",
                objectid.HasValue ? objectid.ToString() : "",
                globalid != null ? globalid : "",
                gdb_archive_oid.HasValue ? gdb_archive_oid.Value.ToString() : "",
                created_user != null ? created_user : "",
                last_edited_user != null ? last_edited_user : "",
            };

            return result;
        }
    }
}
