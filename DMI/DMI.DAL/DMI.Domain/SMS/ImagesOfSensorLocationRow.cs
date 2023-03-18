using System;

namespace DMI.Domain.SMS
{
    public class ImagesOfSensorLocationRow
    {
        public int? objectid { get; set; }
        public DateTime? dateofimage { get; set; }
        public string serviceid { get; set; }
        public string parentguid { get; set; } 
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
