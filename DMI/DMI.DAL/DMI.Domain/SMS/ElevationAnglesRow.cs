using System;
using System.Collections.Generic;
using DMI.Utils;

namespace DMI.Domain.SMS
{
    public class ElevationAnglesRow
    {
        public int? objectid { get; set; }
        public DateTime? datefrom { get; set; }
        public int? angle_n { get; set; }
        public int? angle_ne { get; set; }
        public int? angle_e { get; set; } 
        public int? angle_se { get; set; } 
        public int? angle_s { get; set; } 
        public int? angle_sw { get; set; } 
        public int? angle_w { get; set; } 
        public int? angle_nw { get; set; } 
        public int? angleindex { get; set; }
        public string anglecomment { get; set; }
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

        public static List<string> GenerateHeaderAsListOfString()
        {
            return new List<string>
            {
                "gdb_from_date",
                "gdb_to_date",
                "datefrom",
                "created_date",
                "last_edited_date",
                "angle_n",
                "angle_ne",
                "angle_e",
                "angle_se",
                "angle_s",
                "angle_sw",
                "angle_w",
                "angle_nw",
                "correlation",
                "flipped cor",
                "angleindex calc",
                "angleindex",
                "anglecomment",
                "serviceid",
                "objectid",
                "globalid",
                "parentguid",
                "gdb_archive_oid",
                "created_user",
                "last_edited_user"
            };
        }

        public List<String> AsListOfStrings()
        {
            return new List<string>
            {
                gdb_from_date.HasValue ? gdb_from_date.Value.AsDateTimeString(true) : "",
                gdb_to_date.HasValue ? gdb_to_date.Value.AsDateTimeString(true) : "",
                datefrom.HasValue ? datefrom.Value.AsDateString() : "",
                created_date.HasValue ? created_date.Value.AsDateTimeString(false) : "",
                last_edited_date.HasValue ? last_edited_date.Value.AsDateTimeString(false) : "",
                angle_n.HasValue ? angle_n.Value.ToString() : "",
                angle_ne.HasValue ? angle_ne.Value.ToString() : "",
                angle_e.HasValue ? angle_e.Value.ToString() : "",
                angle_se.HasValue ? angle_se.Value.ToString() : "",
                angle_s.HasValue ? angle_s.Value.ToString() : "",
                angle_sw.HasValue ? angle_sw.Value.ToString() : "",
                angle_w.HasValue ? angle_w.Value.ToString() : "",
                angle_nw.HasValue ? angle_nw.Value.ToString() : "",
                angleindex.HasValue ? angleindex.Value.ToString() : "",
                anglecomment,
                serviceid,
                objectid.HasValue ? objectid.ToString() : "",
                globalid != null ? globalid : "",
                parentguid != null ? parentguid : "",
                gdb_archive_oid.HasValue ? gdb_archive_oid.Value.ToString() : "",
                created_user != null ? created_user : "",
                last_edited_user != null ? last_edited_user : ""
            };
        }
    }
}
