using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMI.Domain.SMS
{
    public class SensorLocationRow
    {
        public int? objectid { get; set; }
        public int? stationid_dmi { get; set; }
        public string accessaddress { get; set; }
        public DateTime? datefrom { get; set; }
        public DateTime? dateto { get; set; }
        public string comment { get; set; }
        public double? barolevel { get; set; }
        public int? status { get; set; }
        public int? sl_utm { get; set; }
        public double? sl_northing { get; set; }
        public double? sl_easting { get; set; }
        public double? sl_geo_lat { get; set; }
        public double? sl_geo_long { get; set; }
        public string globalid { get; set; }
        public string created_user { get; set; }
        public DateTime? created_date { get; set; }
        public string last_edited_user { get; set; }
        public DateTime? last_edited_date { get; set; }
        public int? gdb_archive_oid { get; set; }
        public DateTime? gdb_from_date { get; set; }
        public DateTime? gdb_to_date { get; set; }
    }
}
