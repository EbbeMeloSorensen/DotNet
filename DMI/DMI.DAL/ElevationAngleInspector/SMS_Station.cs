namespace ElevationAngleInspector
{
    public class SMS_Station
    {
        public int? stationid_dmi { get; set; }
        public DateTime datefrom { get; set; }
        public int? angle_n { get; set; }
        public int? angle_ne { get; set; }
        public int? angle_e { get; set; }
        public int? angle_se { get; set; }
        public int? angle_s { get; set; }
        public int? angle_sw { get; set; }
        public int? angle_w { get; set; }
        public int? angle_nw { get; set; }
        public int? angleindex { get; set; }
        public string? anglecomment { get; set; }

        public override string ToString()
        {
            var stationid_dmi_asString = stationid_dmi.HasValue ? stationid_dmi.Value.ToString() : "null";
            var angle_n_asString = angle_n.HasValue ? angle_n.Value.ToString() : "null";
            var angle_ne_asString = angle_ne.HasValue ? angle_ne.Value.ToString() : "null";
            var angle_e_asString = angle_e.HasValue ? angle_e.Value.ToString() : "null";
            var angle_se_asString = angle_se.HasValue ? angle_se.Value.ToString() : "null";
            var angle_s_asString = angle_s.HasValue ? angle_s.Value.ToString() : "null";
            var angle_sw_asString = angle_sw.HasValue ? angle_sw.Value.ToString() : "null";
            var angle_w_asString = angle_w.HasValue ? angle_w.Value.ToString() : "null";
            var angle_nw_asString = angle_nw.HasValue ? angle_nw.Value.ToString() : "null";
            var angleindex_asString = angleindex.HasValue ? angleindex.Value.ToString() : "null";
            var anglecomment_asString = anglecomment != null ? anglecomment : "null";

            return
                $"{stationid_dmi_asString, -10}, " + 
                $"{datefrom.AsShortDateString(), -10}, " + 
                $"{angle_n_asString, 5}, " + 
                $"{angle_ne_asString, 5}, " +
                $"{angle_e_asString, 5}, " +
                $"{angle_se_asString, 5}, " +
                $"{angle_s_asString, 5}, " + 
                $"{angle_sw_asString, 5}, " +
                $"{angle_w_asString, 5}, " +
                $"{angle_nw_asString, 5}, " +
                $"{angleindex_asString, 5}" + 
                $"{anglecomment_asString, 5}";
        }

        public string datefromAsLongDBString()
        {
            var year = datefrom.Year.ToString().PadLeft(2, '0');
            var month = datefrom.Month.ToString().PadLeft(2, '0');
            var day = datefrom.Day.ToString().PadLeft(2, '0');
            var hour = datefrom.Hour.ToString().PadLeft(2, '0');
            var minute = datefrom.Minute.ToString().PadLeft(2, '0');
            var second = datefrom.Second.ToString().PadLeft(2, '0');

            return $"{year}-{month}-{day} {hour}:{minute}:{second}";
        }
    }

    public static class Helpers
    {
        public static string AsShortDateString(this DateTime dateTime)
        {
            var year = dateTime.Year;
            var month = dateTime.Month.ToString().PadLeft(2, '0');
            var day = dateTime.Day.ToString().PadLeft(2, '0');

            return $"{year}-{month}-{day}";
        }
    }
}