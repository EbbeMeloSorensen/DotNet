using System;

namespace DMI.Domain.SMS
{
    public class SensorInformationRow
    {
        public int? objectid { get; set; }
        public int? sensortype { get; set; }
        public DateTime? datefrom { get; set; }
        public DateTime? dateto { get; set; }
        public int? status { get; set; }
        public string make_model { get; set; }
        public string serialnumber { get; set; }
        public double? heightoverterrain { get; set; } 
        public string comment { get; set; } 
        public string serviceid { get; set; } 
        public string calibrationid { get; set; } 
        public string parentguid { get; set; }
        public string globalid { get; set; } 
        public string created_user { get; set; } 
        public DateTime? created_date { get; set; } 
        public string last_edited_user { get; set; } 
        public DateTime? last_edited_date { get; set; }
        public int? gdb_archive_oid { get; set; } 
        public DateTime? gdb_from_date { get; set; } 
        public DateTime? gdb_to_date { get; set; } 
        public string imei { get; set; }
        public DateTime? lastchange { get; set; }
    }
}
